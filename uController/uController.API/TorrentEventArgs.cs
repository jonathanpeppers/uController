using System;
namespace uController.API
{
	public class TorrentEventArgs : EventArgs
	{
		public TorrentEventArgs (Torrent torrent)
		{
			Torrent = torrent;
		}
		
		public Torrent Torrent
		{
			get;
			private set;
		}
	}
}

