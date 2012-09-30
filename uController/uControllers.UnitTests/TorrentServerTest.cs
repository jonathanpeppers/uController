using System;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;
using uController.API;
namespace uController.UnitTests
{
	[TestFixture]
	public class TorrentServerTest
	{
		private const string TorrentUrl = "http://dl.btjunkie.org/torrent/Iron-Man-2-2010-DVDRip-XviD-MAX/435862ebe0e8be1f79f84073579730accc749557c789/download.torrent", 
		    TorrentName = "Iron Man 2 (2010) DVDRip XviD-MAXSPEED";
		private const int Timeout = 10000;
		
		private TorrentServer _server = new TorrentServer
		{
			Url = "jonathanpeppers.dyndns-blog.com",
			Port = 42101,
			Username = "jonathanpeppers",
			Password = "peppers1234",
			Timeout = Timeout,
			PollInterval = 3000,
		};
		
		[Test]
		public void GetTorrents ()
		{
			var response = _server.GetTorrents ();
			
			Assert.IsNotNull (response);
			Assert.IsNotNull (response.Torrents);
			Assert.AreNotEqual (0, response.BuildNumber);
			Assert.AreNotEqual (0, response.CacheID);
		}
		
		[Test]
		public void GetTorrentsWithCacheID ()
		{
			var response = _server.GetTorrents ();
			
			Assert.IsNotNull (response);
			Assert.IsNotNull (response.Torrents);
			Assert.AreNotEqual (0, response.BuildNumber);
			Assert.AreNotEqual (0, response.CacheID);
			
			var cacheResponse = _server.GetTorrents (response.CacheID);
			Assert.IsNotNull (cacheResponse);
			Assert.IsNotNull (cacheResponse.ChangedTorrents);
			Assert.IsNotNull (cacheResponse.RemovedTorrents);
			Assert.AreEqual (response.BuildNumber, cacheResponse.BuildNumber);
			Assert.AreNotEqual (response.CacheID, cacheResponse.CacheID);
		}
		
		[Test]
		public void AddAndRemove ()
		{
			//Add the torrent
			var addResponse = _server.AddUrl (TorrentUrl);
			Assert.IsNotNull (addResponse);
			Assert.AreNotEqual (0, addResponse.BuildNumber);
			
			//Sleep a little bit, takes a little while for the torrent to appear
			Thread.Sleep (5000);
			
			//Check to make sure it added
			var response = _server.GetTorrents ();
			Assert.IsNotNull (response);
			Assert.IsNotNull (response.Torrents);
			Assert.AreNotEqual (0, response.Torrents.Count);
			Assert.AreNotEqual (0, response.BuildNumber);
			Assert.AreNotEqual (0, response.CacheID);
			var torrent = response.Torrents.Values.FirstOrDefault (t => t.Name == TorrentName);
			Assert.IsNotNull (torrent);
			
			//Remove the torrent
			var removeResponse = _server.RemoveData (torrent.ID);
			Assert.IsNotNull (removeResponse);
			Assert.AreNotEqual (0, removeResponse.BuildNumber);
			
			//Check to make sure it's gone
			response = _server.GetTorrents ();
			Assert.IsNotNull (response);
			Assert.IsNotNull (response.Torrents);
			Assert.AreNotEqual (0, response.BuildNumber);
			Assert.AreNotEqual (0, response.CacheID);
			torrent = response.Torrents.Values.FirstOrDefault (t => t.Name == TorrentName);
			Assert.IsNull (torrent);
		}
		
		[Test]
		public void PollingTimer ()
		{
			AutoResetEvent resetEvent = new AutoResetEvent (false);
			EventHandler handler = (sender, e) =>
			{
				resetEvent.Set ();
			};
			
			try
			{
				_server.TorrentsChanged += handler;
				_server.StartPolling ();
				Assert.IsTrue (resetEvent.WaitOne (Timeout));
				Assert.IsNotNull (_server.Torrents);
				
				//Add another torrent
				var addResponse = _server.AddUrl (TorrentUrl);
				Assert.IsNotNull (addResponse);
				Assert.AreNotEqual (0, addResponse.BuildNumber);		
				
				//Assert the torrent is there
				Assert.IsTrue (resetEvent.WaitOne (Timeout));
				Assert.IsNotNull (_server.Torrents);
				Assert.AreNotEqual (0, _server.Torrents.Length);
				var torrent = _server.Torrents.FirstOrDefault (t => t.Name == TorrentName);
				Assert.IsNotNull (torrent);
				
				//Delete the torrent
				var removeResponse = _server.Remove (torrent.ID);
				Assert.IsNotNull (removeResponse);
				Assert.AreNotEqual (0, removeResponse.BuildNumber);
				
				//Assert the torrent is gone
				Assert.IsTrue (resetEvent.WaitOne (Timeout));
				Assert.IsNotNull (_server.Torrents);
				torrent = _server.Torrents.FirstOrDefault (t => t.Name == TorrentName);
				Assert.IsNull (torrent);
			}
			finally
			{
				_server.StopPolling ();
				_server.TorrentsChanged -= handler;
			}
		}
	}
}

