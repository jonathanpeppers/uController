using System;
using System.Drawing;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API;
using uController.Models;
namespace uController.Controllers
{
	[Register("TorrentDetailsDelegate")]
	public class TorrentDetailsDelegate : UITableViewSource
	{
		private Server _server = null;
		private UITableViewCell[] _cells = new UITableViewCell[9];
		private ITinyMessengerHub _hub = null;
		private Torrent _torrent = null;
		private UIActionSheet _sheet = null;
		
		#region Constructors
	
		public TorrentDetailsDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public TorrentDetailsDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_server = container.Resolve<Server>();
			_hub = container.Resolve<ITinyMessengerHub>();
			
			_hub.Subscribe<TorrentDetailsMessage>(m =>
			{
				_torrent = m.Torrent;	
				_torrentDetailsController.TableView.ReloadData();
				_torrentDetailsController.TableView.SetContentOffset(PointF.Empty, false);
				_navigationController.PushViewController(_torrentDetailsController, true);
			});
			_hub.Subscribe<TorrentChangedMessage>(m => Execute.BeginInvoke(() =>
			{
				if (_torrent != null && _torrent.ID == m.Torrent.ID)
				{
					if (m.Removed)	
					{
						_navigationController.PopViewControllerAnimated(true);
					}
					else
					{
						_torrent = m.Torrent;
						_torrentDetailsController.TableView.ReloadData();
					}
				}
			}));
			
			
			for (int i = 0; i < _cells.Length; i++)
			{
				_cells[i] = new UITableViewCell(UITableViewCellStyle.Value1, string.Empty)
				{
					SelectionStyle = UITableViewCellSelectionStyle.None,	
				};
			}
		}
		
		#endregion
		
		[Outlet]
		private UITabBar _tabBar
		{
			get; set;
		}
		
		[Outlet]
		private UINavigationController _navigationController
		{
			get; set;
		}
		
		[Outlet]
		private UITableViewController _torrentDetailsController
		{
			get; set;
		}
		
		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return _torrent == null ? string.Empty : _torrent.Name;
		}
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			return _cells.Length;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			if (indexPath.Row == 0)
			{
				_sheet = new UIActionSheet("Change Torrent Status", null, "Cancel", "Remove Torrent + Data", "Remove");
				_sheet.Dismissed += OnDimissed;
				_sheet.ShowFromTabBar(_tabBar);
			}
		}

		private void OnDimissed(object sender, UIButtonEventArgs e)
		{
			_sheet.Dismissed -= OnDimissed;
			_sheet.Dispose ();
			_sheet = null;
			_cells[0].Selected = false;
			
			switch (e.ButtonIndex)
			{
				case 0:
					SendToServer(() => _server.RemoveData(_torrent.ID));
					_navigationController.PopViewControllerAnimated(true);
					break;
				case 1:
					SendToServer(() => _server.Remove(_torrent.ID));
					_navigationController.PopViewControllerAnimated(true);
					break;
				default:
					break;
			}
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = _cells[indexPath.Row];
			
			if (_torrent != null)
			{
				switch (indexPath.Row)
				{
					case 0:
						cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
						cell.TextLabel.Text = "Status";
						cell.DetailTextLabel.Text = _torrent.StatusString;
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
						break;
					case 1:
						cell.TextLabel.Text = "Size";
						cell.DetailTextLabel.Text = _torrent.SizeString;
						break;
					case 2:
						cell.TextLabel.Text = "Progress";
						cell.DetailTextLabel.Text = _torrent.PercentProgress / 10 + " %";
						break;
					case 3:
						cell.TextLabel.Text = "Uploaded";
						cell.DetailTextLabel.Text = _torrent.UploadedString;
						break;
					case 4:
						cell.TextLabel.Text = "Downloaded";
						cell.DetailTextLabel.Text = _torrent.DownloadedString;
						break;
					case 5:
						cell.TextLabel.Text = "Speed";
						cell.DetailTextLabel.Text = _torrent.UpBps + ", " + _torrent.DownBps;
						break;
					case 6:
						cell.TextLabel.Text = "Remaining";
						cell.DetailTextLabel.Text = _torrent.EtaString;
						break;
					case 7:
						cell.TextLabel.Text = "Seeds";
						cell.DetailTextLabel.Text = _torrent.SeedsConnected + " / " + _torrent.SeedsInSwarm;
						break;
					case 8:
						cell.TextLabel.Text = "Peers";
						cell.DetailTextLabel.Text = _torrent.PeersConnected + " / " + _torrent.PeersInSwarm;
						break;
					default:
						break;
				}	
			}
			
			return cell;
		}
		
		private void SendToServer(Action action)
		{
			Execute.Background(() =>
			{
				try
				{
					action();
					_server.StartPolling();
				}
				catch (Exception exc)
				{
					Execute.BeginInvoke(() => MessageBox.Show(exc.Message, "Error"));	
				}
			});
		}
	}
}

