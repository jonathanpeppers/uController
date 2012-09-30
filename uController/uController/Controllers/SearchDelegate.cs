using System;
using System.Collections.Generic;
using System.iPhone.IoC;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
using uController.Models;
using System.iPhone.UIKit;
namespace uController.Controllers
{
	[Register("SearchDelegate")]
	public class SearchDelegate : UITableViewSource
	{
		private ITinyMessengerHub _hub = null;
		private ISearchProvider[] _providers = null;
		private ISearchProvider _provider = null;
		private BusyIndicator _busyIndicator = null;
		private UITableView _tableView = null;
		private Dictionary<int, SearchResultCellController> _controllers = new Dictionary<int, SearchResultCellController>();
		private TorrentUrl[] _torrents = null;
		private int _id = 0;

		#region Constructors
	
		public SearchDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public SearchDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_hub = container.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<SearchMessage>(OnSearch);
			_busyIndicator = container.Resolve<BusyIndicator>();
			_providers = container.ResolveAll<ISearchProvider>().ToArray();
			foreach (ISearchProvider item in _providers)
			{
				item.SearchCompleted += OnSearchCompleted;
			}
		}

		#endregion

		private void OnSearch(SearchMessage message)
		{
			if (string.IsNullOrEmpty(message.SearchText))
			{
				if (_torrents != null)
				{
					_torrents = null;
					if (_tableView != null)
					{
						_tableView.ReloadData();
					}
				}
			}
			else
			{
				_busyIndicator.IsBusy = true;
				
				_provider = _providers[message.SearchScope];
				_provider.Search(message.SearchText);
			}
		}

		private void OnSearchCompleted (object sender, SearchEventArgs e)
		{
			Execute.BeginInvoke(() =>
			{
				if (e.Success)
				{
					_torrents = e.Urls;
					if (_tableView != null)
					{
						_tableView.ReloadData();
					}
				}
				_busyIndicator.IsBusy = false;
			});
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			_tableView = tableview;

			return _torrents == null ? 0 : _torrents.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell("SearchResultCell");
			SearchResultCellController controller = null;
			if (cell == null)
			{
				controller = new SearchResultCellController();
				cell = controller.Load(_torrents[indexPath.Row]);
				cell.Tag = _id++;
				_controllers[cell.Tag] = controller;
			}
			else
			{
				controller = _controllers[cell.Tag];
				controller.Load(_torrents[indexPath.Row]);
			}
			
			return cell;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_hub.Publish(new SearchDetailsMessage(this, _torrents[indexPath.Row], _provider));
		}
	}
}

