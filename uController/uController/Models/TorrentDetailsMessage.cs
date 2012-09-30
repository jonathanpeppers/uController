using System;
using System.iPhone.IoC;
using uController.API;
namespace uController.Models
{
	public class TorrentDetailsMessage : TinyMessageBase
	{
		public TorrentDetailsMessage (object sender, Torrent torrent)
			: base (sender)
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

