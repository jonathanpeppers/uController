using System;
using System.Json;
namespace uController.API
{
	public class Torrent
	{
		public Torrent (JsonValue json)
		{
			if (json.Count == 1)
			{
				ID = json[0];
				return;
			}
			
			int index = 0;
			ID = json[index++];
			Status = (TorrentStatus)(int)json[index++];
			Name = json[index++];
			Size = json[index++];
			PercentProgress = json[index++];
			Downloaded = json[index++];
			Uploaded = json[index++];
			Ratio = json[index++];
			UploadSpeed = json[index++];
			DownloadSpeed = json[index++];
			Eta = json[index++];
			Label = json[index++];
			PeersConnected = json[index++];
			PeersInSwarm = json[index++];
			SeedsConnected = json[index++];
			SeedsInSwarm = json[index++];
			Availability = json[index++];
			QueueOrder = json[index++];
			Remaining = json[index++];
		}
		
		public string ID
		{
			get;
			set;
		}
		
		/// <summary>
		/// The STATUS is a bitwise value, which is obtained by adding up the different values for corresponding statuses:
		/// For example, if a torrent job has a status of 201 = 128 + 64 + 8 + 1, then it is loaded, queued, checked, and started. A bitwise AND operator should be used to determine whether the given STATUS contains a particular status.
		/// </summary>
		public TorrentStatus Status
		{
			get;
			set;
		}
		
		public string Name
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes
		/// </summary>
		public long Size
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in per mils
		/// </summary>
		public long PercentProgress
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes
		/// </summary>
		public long Downloaded
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes
		/// </summary>
		public long Uploaded
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in per mils
		/// </summary>
		public long Ratio
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes per second
		/// </summary>
		public long UploadSpeed
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes per second
		/// </summary>
		public long DownloadSpeed
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in seconds
		/// </summary>
		public long Eta
		{
			get;
			set;
		}
		
		public string Label
		{
			get;
			set;
		}
		
		public long PeersConnected
		{
			get;
			set;
		}
		
		public long PeersInSwarm
		{
			get;
			set;
		}
		
		public long SeedsConnected
		{
			get;
			set;
		}
		
		public long SeedsInSwarm
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in 1/65536ths
		/// </summary>
		public long Availability
		{
			get;
			set;
		}
		
		public long QueueOrder
		{
			get;
			set;
		}
		
		/// <summary>
		/// integer in bytes
		/// </summary>
		public long Remaining
		{
			get;
			set;
		}
		
		#region Readonly helper fields
		
		public TimeSpan EtaAsTimeSpan
		{
			get { return TimeSpan.FromSeconds(Eta); }
		}
		
		public string EtaString
		{
			get 
			{ 
				if (Eta == 0)
				{
					return string.Empty;	
				}
				if (Eta < 0)
				{
					return "ETA: ∞";	
				}
				return "ETA: " + EtaAsTimeSpan;
			}
		}
		
		public string StatusString
		{
			get
			{
				if ((Status & TorrentStatus.Paused) != 0)
				{
					return "Paused";
				}
				if ((Status & TorrentStatus.Started) != 0)
				{
					return PercentProgress == 1000 ? "Seeding" : "Downloading";
				}
				if ((Status & TorrentStatus.Checking) != 0)
				{
					return "Checking";
				}
				if ((Status & TorrentStatus.Queued) != 0)
				{
					return PercentProgress == 1000 ? "Queued Seed" : "Queued";
				}
				if ((Status & TorrentStatus.Error) != 0)
				{
					return "Error";
				}
	
				return PercentProgress == 1000 ? "Finished" : "Stopped";
			}
		}
		
		public string UpBps
		{
			get { return ToBps("Up: ", UploadSpeed, "/s"); }
		}
		
		public string DownBps
		{
			get { return ToBps("Down: ", DownloadSpeed, "/s"); }
		}
		
		public string SizeString
		{
			get { return ToBps(string.Empty, Size, string.Empty); }
		}
		
		public string DownloadedString
		{
			get { return ToBps(string.Empty, Downloaded, string.Empty); }
		}
		
		public string UploadedString
		{
			get { return ToBps(string.Empty, Uploaded, string.Empty); }
		}
			
		private string ToBps(string label, long bps, string extra)
		{
			const int KB = 1024,
				MB = KB * KB,
				GB = KB * KB * KB;
			
			if (bps > GB)
				return string.Format("{0}{1} GB{2}", label, bps / GB, extra);
			if (bps > MB)
				return string.Format("{0}{1} MB{2}", label, bps / MB, extra);
			if (bps > KB)
				return string.Format("{0}{1} KB{2}", label, bps / KB, extra);

			return string.Format("{0}{1} B{2}", label, bps, extra);
		}
		
		#endregion
	}
}

