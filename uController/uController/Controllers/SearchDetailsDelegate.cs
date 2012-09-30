using System;
using System.Drawing;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
using uController.Models;
namespace uController.Controllers
{
	[Register("SearchDetailsDelegate")]
	public class SearchDetailsDelegate : UITableViewSource
	{
		private ITinyMessengerHub _hub = null;
		private BusyIndicator _busyIndicator = null;
		private TorrentUrlDetails _details = null;
		private UITableViewCell[] _cells = new UITableViewCell[8];
		
		#region Constructors
	
		public SearchDetailsDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public SearchDetailsDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_hub = container.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<SearchDetailsMessage>(OnDetails);
			_busyIndicator = container.Resolve<BusyIndicator>();
			
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
		private UINavigationController _navigationController
		{
			get; set;
		}
		
		[Outlet]
		private UITableViewController _searchDetailsController
		{
			get; set;
		}
		
		private void OnDetails(SearchDetailsMessage message)
		{
			_busyIndicator.IsBusy = true;
			_details = null;
			_searchDetailsController.TableView.ReloadData();
			_searchDetailsController.TableView.SetContentOffset(PointF.Empty, false);
			_navigationController.PushViewController(_searchDetailsController, true);
			
			//Fire off the search
			message.Provider.ExtendedSearchCompleted += OnExtendedSearch;
			message.Provider.ExtendedSearch(message.Url);
		}
		
		private void OnExtendedSearch(object sender, ExtendedSearchEventArgs e)
		{
			Execute.BeginInvoke(() =>
			{
				try
				{
					var provider = (ISearchProvider)sender;
					provider.ExtendedSearchCompleted -= OnExtendedSearch;
					if (e.Success)
					{
						_details = e.TorrentDetails;
						_searchDetailsController.TableView.ReloadData();
					}
					else
					{
						MessageBox.Show(e.ErrorMessage, "Error");
					}
					_busyIndicator.IsBusy = false;
				}
				catch (Exception exc)
				{
					Console.WriteLine(exc);	
				}
			});
		}
		
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return _details == null ? string.Empty : _details.TorrentUrl.NameTrimmed;
		}
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			return _details == null ? 0 : _cells.Length;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var cell = _cells[indexPath.Row];
			
			if (_details != null)
			{
				switch (indexPath.Row)
				{
					case 0:
						cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
						cell.TextLabel.Text = "Download";
						cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
						break;
					case 1:
						if (_details.Comments == null || _details.Comments.Length == 0)
						{
							cell.DetailTextLabel.Text = "None";
							cell.Accessory = UITableViewCellAccessory.None;
							cell.SelectionStyle = UITableViewCellSelectionStyle.None;	
						}
						else
						{
							cell.DetailTextLabel.Text = string.Empty;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;	
						}
						cell.TextLabel.Text = "Comments";
						break;
					case 2:
						if (_details.Files == null || _details.Files.Length == 0)
						{
							cell.DetailTextLabel.Text = "N/A";
							cell.Accessory = UITableViewCellAccessory.None;
							cell.SelectionStyle = UITableViewCellSelectionStyle.None;
						}
						else
						{
							cell.DetailTextLabel.Text = string.Empty;
							cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
							cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
						}
						cell.TextLabel.Text = "Files";
						break;
					case 3:
						cell.TextLabel.Text = "Seeds / Peers";
						cell.DetailTextLabel.Text = _details.Seeds + " / " + _details.Peers;
						break;
					case 4:
						cell.TextLabel.Text = "Created By";
						cell.DetailTextLabel.Text = _details.CreatedBy;
						break;
					case 5:
						cell.TextLabel.Text = "Date Created";
						cell.DetailTextLabel.Text = _details.CreatedOn.ToString("MM/dd/yyyy");
						break;
					case 6:
						cell.TextLabel.Text = "Category";
						cell.DetailTextLabel.Text = _details.TorrentUrl.CategoryTrimmed;
						break;
					case 7:
						cell.TextLabel.Text = "Size";
						cell.DetailTextLabel.Text = _details.TorrentUrl.SizeTrimmed;
						break;
					default:
						break;
				}
			}
			return cell;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			switch (indexPath.Row)
			{
				case 0:
					_hub.Publish(new AddTorrentMessage(this, _details.TorrentUrl));
					break;
				case 1:
					if (_details.Comments != null && _details.Comments.Length != 0)
					{
						_hub.Publish(new CommentsMessage(this, _details));
					}
					break;
				case 2:
					if (_details.Files != null && _details.Files.Length != 0)
					{
						_hub.Publish(new FilesMessage(this, _details));
					}
					break;
				default:
					break;
			}
		}
	}
}

