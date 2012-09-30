using System;
using System.iPhone.IoC;
using uController.API;
namespace uController.Models
{
	public class TorrentChangedMessage : TinyMessageBase
	{
		public TorrentChangedMessage (object sender, bool removed, Torrent torrent)
			: base(sender)
		{
			Removed = removed;
			Torrent = torrent;
		}
		
		public bool Removed
		{
			get;
			private set; 
		}
		
		public Torrent Torrent
		{
			get;
			private set; 
		}
	}
}

