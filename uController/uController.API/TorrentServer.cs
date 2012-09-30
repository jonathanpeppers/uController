using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Json;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Timers;
namespace uController.API
{
	public class TorrentServer
	{
		public event EventHandler<TorrentEventArgs> TorrentChanged = delegate { };
		public event EventHandler<TorrentEventArgs> TorrentRemoved = delegate { };
		public event EventHandler TorrentsChanged = delegate { };
		public event EventHandler<TorrentErrorEventArgs> TorrentsError = delegate { };
		
		private static readonly Regex _tokenRegex = new Regex(@"\<html\>\<div id='token' style='display:none;'\>(.+)\</div\>\</html\>", RegexOptions.IgnoreCase);
		private bool _stopped = true;
		private Timer _timer = new Timer();
		private Dictionary<string, Torrent> _torrents = new Dictionary<string, Torrent>();
		private string _cacheID = string.Empty;
		
		public virtual int Timeout
		{
			get;
			set;
		}
		
		public virtual string Username
		{
			get;
			set;
		}
		
		public virtual string Password
		{
			get;
			set;
		}
		
		public virtual string Url
		{
			get;
			set;
		}
		
		public virtual int Port
		{
			get;
			set;
		}
		
		public virtual double PollInterval
		{
			get;
			set;
		}
		
		public Torrent[] Torrents
		{
			get;
			private set; 
		}
		
		public string Token
		{
			get;
			private set;
		}
		
		public TorrentServer ()
		{
			PollInterval = 5000;
			Timeout = 10000;
			
			_timer.AutoReset = false;
			_timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
		}
		
		private void _timer_Elapsed (object sender, ElapsedEventArgs e)
		{
			lock (_timer)
			{
				_timer.Stop ();
				
				try
				{
					bool changed = false;
					var response = string.IsNullOrEmpty(_cacheID) ? GetTorrents() : GetTorrents(_cacheID);
					
					if (response.ChangedTorrents != null)
					{
						foreach (var item in response.ChangedTorrents)
						{
							changed = true;

							_torrents[item.ID] = item;
							
							TorrentChanged(this, new TorrentEventArgs(item));
						}
					}
					
					if (response.RemovedTorrents != null)
					{
						foreach (var item in response.RemovedTorrents)
						{
							changed = true;

							_torrents.Remove (item.ID);
							
							TorrentRemoved(this, new TorrentEventArgs(item));
						}
					}
					
					if (response.Torrents != null)
					{
						//First check for removed torrents
						var array = _torrents.Values.ToArray();
						foreach (var item in array)
						{
							if (!response.Torrents.ContainsKey(item.ID))
							{
								changed = true;
								
								_torrents.Remove(item.ID);
								
								TorrentRemoved(this, new TorrentEventArgs(item));
							}
						}
						
						//Now check for changed/new torrents
						foreach (var item in response.Torrents.Values)
						{
							changed = true;

							_torrents[item.ID] = item;
							
							TorrentChanged(this, new TorrentEventArgs(item));
						}
					}
					
					_cacheID = response.CacheID;
					
					if (changed)
					{
						Torrents = _torrents.Values.ToArray ();
						TorrentsChanged (this, EventArgs.Empty);
					}
				}
				catch (WebException exc)
				{
					TorrentsError(this, new TorrentErrorEventArgs(exc));
					return;
				}
				catch (InvalidOperationException)
				{
					Console.WriteLine("JSON Exception");
					
					//This means a JSON problem with the stream, so just retry & blank out the past response
					_cacheID = string.Empty;
					System.Threading.ThreadPool.QueueUserWorkItem(_ => _timer_Elapsed(sender, e));
				}
				catch (Exception exc)
				{
					TorrentsError(this, new TorrentErrorEventArgs(exc));
					return;
				}
				
				if (!_stopped)
				{
					_timer.Start();
				}
			}
		}
		
		private void GetToken ()
		{
			if (string.IsNullOrEmpty (Token))
			{
				//<html><div id='token' style='display:none;'>dgVpm4K4acYlJdI20Dyefk4h-cMmm-ooJw3KK9V_oOYxQcS1-K1-Ns1g0Uw=</div></html>
				string token = string.Empty;
				MakeRequest ("token.html", stream => { using (StreamReader reader = new StreamReader(stream)) token = reader.ReadToEnd(); }, false);
				Token = _tokenRegex.Match(token).Groups[1].Value;
				Console.WriteLine("TOKEN: " + Token);
			}
		}
		
		private void MakeRequest (string action, Action<Stream> handler, bool useToken)
		{
			MakeRequest (action, handler, useToken, false);
		}
		
		private void MakeRequest (string action, Action<Stream> handler, bool useToken, bool retry)
		{
			string url = null;
			if (useToken)
			{
				GetToken ();
				url = string.Format ("http://{0}:{1}/gui/?token={2}&{3}", Url, Port, Token, action);
			}
			else
			{
				url = string.Format ("http://{0}:{1}/gui/{2}", Url, Port, action);
			}
			
			try
			{
				HttpWebRequest request = HttpWebRequest.Create (url) as HttpWebRequest;
				request.Timeout = Timeout;
				request.Method = WebRequestMethods.Http.Get;
				request.Credentials = new NetworkCredential (Username, Password);
				request.PreAuthenticate = true;
				using (HttpWebResponse response = request.GetResponse () as HttpWebResponse)
				{
					using (Stream stream = response.GetResponseStream ())
					{
						handler (stream);
					}
				}
			}
			catch (WebException exc)
			{
				if (exc.Response == null)
				{
					throw exc;
				}
				
				using (HttpWebResponse response = exc.Response as HttpWebResponse)
				{
					//This means the token is bad
					if (!retry && (response.StatusCode == HttpStatusCode.Ambiguous || response.StatusCode == HttpStatusCode.BadRequest))
					{
						Token = null;
						MakeRequest (action, handler, useToken, true);
					}
					else
					{
						Exception toThrow = exc;
						try
						{
							using (Stream stream = response.GetResponseStream ())
							{
								using (StreamReader reader = new StreamReader (stream))
								{
									string text = reader.ReadToEnd ();
								
									toThrow = new Exception (string.Format ("{0} Error, {1}: {2}", (int)response.StatusCode, response.StatusCode, text));
								}
							}
						}
						catch
						{ }
						throw toThrow;
					}
				}
			}
		}
		
		public ListResponse GetTorrents ()
		{
			JsonValue json = null;
			MakeRequest("list=1", stream => json = JsonObject.Load(stream), true);	
			return new ListResponse(json);
		}
		
		public ListResponse GetTorrents (string cacheID)
		{
			JsonValue json = null;
			MakeRequest ("list=1&cid=" + cacheID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse AddUrl (string torrentUrl)
		{
			JsonValue json = null;
			MakeRequest ("action=add-url&s=" + HttpUtility.UrlEncode (torrentUrl), stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse Start (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=start&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse Stop (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=stop&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse Pause (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=pause&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse Unpause (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=unpause&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse ForceStart (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=forcestart&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse Remove (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=remove&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public ListResponse RemoveData (string torrentID)
		{
			JsonValue json = null;
			MakeRequest ("action=removedata&hash=" + torrentID, stream => json = JsonObject.Load (stream), true);
			return new ListResponse (json);
		}
		
		public void StartPolling()
		{
			_stopped = false;
			
			_timer.Interval = PollInterval;
			//This fires the timer immediately
			_timer_Elapsed(null, null);
		}
		
		public void StopPolling()
		{
			_stopped = true;
			_timer.Stop();	
		}
	}
}

