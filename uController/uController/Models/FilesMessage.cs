using System;
using System.iPhone.IoC;
using uController.API.Search;
namespace uController.Models
{
	public class FilesMessage : TinyMessageBase
	{
		public FilesMessage (object sender, TorrentUrlDetails details)
			: base(sender)
		{
			Details = details;
		}
		
		public TorrentUrlDetails Details
		{
			get;
			private set;
		}
	}
}

