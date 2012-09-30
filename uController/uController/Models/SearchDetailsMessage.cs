using System;
using System.iPhone.IoC;
using uController.API.Search;
namespace uController.Models
{
	public class SearchDetailsMessage : TinyMessageBase
	{
		public SearchDetailsMessage (object sender, TorrentUrl url, ISearchProvider provider)
			: base(sender)
		{
			Url = url;
			Provider = provider;
		}
		
		public TorrentUrl Url
		{
			get;
			private set;
		}
		
		public ISearchProvider Provider
		{
			get;
			private set;
		}
	}
}

