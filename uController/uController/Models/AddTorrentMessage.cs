using System;
using System.iPhone.IoC;
using uController.API.Search;
namespace uController.Models
{
	public class AddTorrentMessage : TinyMessageBase
	{
		public AddTorrentMessage (object sender, TorrentUrl url)
			: base(sender)
		{
			Url = url;	
		}
		
		public TorrentUrl Url
		{
			get;
			private set;
		}
	}
}

