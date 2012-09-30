using System;
using System.Linq;
using System.Web;
namespace uController.API.Search
{
	public class MiniNovaProvider : RssProvider
	{
		#region implemented abstract members of uController.API.Search.RssProvider

		protected override string BuildUrl (string searchText)
		{
			return "http://www.mininova.org/rss/" + HttpUtility.UrlEncode(searchText.Replace(' ', '+'));
		}
		
		
		protected override void OnRssCompleted (Rss rss)
		{
			OnSearchCompleted(new SearchEventArgs(rss.Channel.Items.Select(i => new TorrentUrl { Url = i.Link, Name = i.Title }).ToArray()));
		}
		
		#endregion
	}
}

