using System;
using System.Collections.Generic;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API;
using uController.Models;
namespace uController.Controllers
{
	[Register("TorrentsDelegate")]
	public class TorrentsDelegate : UITableViewSource
	{
		private ITinyMessengerHub _hub = null;
		private Server _server = null;
		private Settings _settings = null;
		private Dictionary<int, TorrentCellController> _controllers = new Dictionary<int, TorrentCellController>();
		private int _id = 0;

		#region Constructors
	
		public TorrentsDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public TorrentsDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			
			//Messenger
			_hub = container.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<ApplicationMessage>(m => 
			{ 
				if (m.Content == ApplicationMessageType.Loaded || m.Content == ApplicationMessageType.Foregrounded)  
				{
//					if (!string.IsNullOrEmpty(_settings.Url) && ++_settings.NumberOfRuns == 3)
//					{
//						MessageBox.Ask("Would you consider donating a mere $.99?", "Enjoy the App?", yes =>
//						{
//							if (yes)
//							{
//								_hub.Publish(new DonateMessage(this));	
//							}
//							else
//							{
//								Refresh();	
//							}
//						});
//					}
//					else
					{
						Refresh();
					}
				}
			});
			_hub.Subscribe<AddTorrentMessage>(m =>
			{
				try
				{
					_server.AddUrl(m.Url.Url);
				}
				catch (Exception exc)
				{
					Execute.BeginInvoke(() => MessageBox.Show(exc.Message, "Error"));
				}
			});
			_settings = container.Resolve<Settings>();
			
			//Server
			_server = container.Resolve<Server>();
			_server.TorrentsChanged += (sender, e) => Execute.BeginInvoke(() =>
			{
				if (_tableView != null)
				{
					_tableView.ReloadData();
				}
			});
			_server.TorrentsError += (sender, e) => Execute.BeginInvoke(() =>
			{
				MessageBox.Show("Could not get list of torrents: " + Environment.NewLine +
				                e.Exception.Message + Environment.NewLine +
				                "Press refresh to try again.", "Error");
			});
			_server.TorrentChanged += (sender, e) => _hub.Publish(new TorrentChangedMessage(this, false, e.Torrent));
			_server.TorrentRemoved += (sender, e) => _hub.Publish(new TorrentChangedMessage(this, true, e.Torrent));
		}
		#endregion
		
		[Outlet]
		private UITableView _tableView
		{
			get; set;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			_hub.Publish(new TorrentDetailsMessage(this, _server.Torrents[indexPath.Row]));
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _server.Torrents == null ? 0 : _server.Torrents.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell("TorrentsCell");
			var torrent = _server.Torrents[indexPath.Row];
			TorrentCellController controller = null;
			if (cell == null)
			{
				controller = new TorrentCellController();
				cell = controller.Load(torrent);
				cell.Tag = _id++;
				_controllers[cell.Tag] = controller;
			}
			else
			{
				controller = _controllers[cell.Tag];
				controller.Load(torrent);
			}
			
			return cell;
		}
		
		[Export("Refresh:")]
		public void Refresh()
		{
			if (string.IsNullOrEmpty(_settings.Url))
			{
				_hub.Publish(new EmptyUrlMessage(this));	
			}
			else
			{
				Execute.Background(_server.StartPolling);
			}
		}
	}
}

