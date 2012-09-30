using System;
using System.Drawing;
using System.iPhone.IoC;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
using uController.Models;
using System.iPhone.UIKit;
namespace uController.Controllers
{
	[Register("FilesDelegate")]
	public class FilesDelegate : UITableViewSource
	{
		private ITinyMessengerHub _hub = null;
		private TorrentUrlDetails _details = null;
		
		[Outlet]
		private UINavigationController _navigationController
		{
			get; set;
		}
		
		[Outlet]
		private UITableViewController _filesController
		{
			get; set;
		}
		
		#region Constructors
	
		public FilesDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public FilesDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_hub = container.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<FilesMessage>(m => 
			{
				_details = m.Details;
				_filesController.TableView.ReloadData();
				_filesController.TableView.SetContentOffset(PointF.Empty, false);
				_navigationController.PushViewController(_filesController, true);	
			});
		}
		#endregion
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			return _details == null || _details.Files == null ? 0 : _details.Files.Length;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var file = _details.Files[indexPath.Row];
			var cell = tableView.DequeueReusableCell("FileCell");
			if (cell == null)
			{
				cell = new UITableViewCell(UITableViewCellStyle.Value2, "FileCell");
			}
			cell.TextLabel.Text = file.Size;
			cell.DetailTextLabel.Text = file.Name;
			cell.SelectionStyle = UITableViewCellSelectionStyle.Gray;
			return cell;
		}
		
		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			var file = _details.Files[indexPath.Row];
			MessageBox.Show(file.Name, "Filename");
		}
	}
}

