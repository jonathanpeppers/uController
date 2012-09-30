using System;
using System.Threading;
using NUnit.Framework;
using uController.API.Search;
namespace uControllers.UnitTests
{
	[TestFixture]
	public class BtJunkieProviderTest
	{
		private BtJunkieProvider _provider = new BtJunkieProvider();
		
		private TorrentUrl[] TestSearch(string searchText)
		{
			
			AutoResetEvent resetEvent = new AutoResetEvent(false);
			TorrentUrl[] torrents = null;
			string error = null;
			
			EventHandler<SearchEventArgs> handler = (sender, e) => 
			{
				if (e.Success)
				{
					torrents = e.Urls;
				}
				else
				{
					error = e.ErrorMessage;
				}

				resetEvent.Set();
			};
			
			try
			{
				_provider.SearchCompleted += handler;
				_provider.Search(searchText);
	
				resetEvent.WaitOne(30000);
				
				Assert.IsNull(error, error);
				Assert.IsNotNull(torrents);
				Assert.AreNotEqual(0, torrents.Length);
	
				return torrents;
			}
			finally
			{
				_provider.SearchCompleted -= handler;
			}
		}
		
		private TorrentUrlDetails TestSearchDetails(TorrentUrl url)
		{
			
			AutoResetEvent resetEvent = new AutoResetEvent(false);
			TorrentUrlDetails details = null;
			string error = null;
			
			EventHandler<ExtendedSearchEventArgs> handler = (sender, e) => 
			{
				if (e.Success)
				{
					details = e.TorrentDetails;
				}
				else
				{
					error = e.ErrorMessage;
				}

				resetEvent.Set();
			};
			
			try
			{
				_provider.ExtendedSearchCompleted += handler;
				_provider.ExtendedSearch(url);
	
				resetEvent.WaitOne(30000);
				
				Assert.IsNull(error, error);
	
				return details;
			}
			finally
			{
				_provider.ExtendedSearchCompleted -= handler;
			}
		}

		//[Test]
		public void GetTorrents()
		{
			var torrents = TestSearch("Iron Man 2");
			
			foreach (var item in torrents)
			{
				Assert.IsFalse(string.IsNullOrEmpty(item.Url));
				Assert.IsFalse(string.IsNullOrEmpty(item.Name));
			}
		}

		//[Test]
		public void GetTorrentsWithBadChars()
		{
			var torrents = TestSearch("Tom & Jerry");
			
			foreach (var item in torrents)
			{
				Assert.IsFalse(string.IsNullOrEmpty(item.Url));
				Assert.IsFalse(string.IsNullOrEmpty(item.Name));
			}
		}
		
		[Test]
		public void GetTorrentDetails()
		{
			var torrents = TestSearch("Iron Man 2");
			
			foreach (var item in torrents)
			{
				Assert.IsFalse(string.IsNullOrEmpty(item.Url));
				Assert.IsFalse(string.IsNullOrEmpty(item.Name));
			}
			
			//Now let's get the details
			var details = TestSearchDetails(torrents[0]);
			
			Assert.IsNotNull(details);
			Assert.IsNotEmpty(details.CreatedBy);
			Assert.AreNotEqual(0, details.Seeds);
			Assert.AreNotEqual(0, details.Peers);
			Assert.AreNotEqual(default(DateTime), details.CreatedOn);
			Assert.IsNotNull(details.Files);
			Assert.AreNotEqual(0, details.Files.Length);
			foreach (var item in details.Files)
			{
				Assert.IsNotEmpty(item.Name);
				Assert.IsNotEmpty(item.Size);
			}
			Assert.IsNotNull(details.Comments);
			Assert.AreNotEqual(0, details.Comments.Length);
			foreach (var item in details.Comments)
			{
				Assert.IsNotEmpty(item.UserInfo);
				Assert.IsNotEmpty(item.User);
				Assert.IsNotEmpty(item.Text);
				Assert.IsNotEmpty(item.CommentInfo);
			}
		}
	}
}

