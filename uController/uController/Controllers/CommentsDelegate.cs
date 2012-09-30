using System;
using System.Collections.Generic;
using System.Drawing;
using System.iPhone.IoC;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
using uController.Models;
namespace uController.Controllers
{
	[Register("CommentsDelegate")]
	public class CommentsDelegate : UITableViewSource
	{
		private readonly SizeF _size = new SizeF(280, 1000);
		private ITinyMessengerHub _hub = null;
		private Dictionary<int, CommentCellController> _controllers = new Dictionary<int, CommentCellController>();
		private int _id = 0;
		private TorrentUrlDetails _details = null;
		private UIFont _font = UIFont.FromName("Helvetica", 14f);
		
		#region Constructors
	
		public CommentsDelegate (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public CommentsDelegate (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_hub = container.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<CommentsMessage>(m => 
			{
				_details = m.Details;
				_commentsController.TableView.ReloadData();
				_commentsController.TableView.SetContentOffset(PointF.Empty, false);
				_navigationController.PushViewController(_commentsController, true);	
			});
		}
		#endregion
		
		[Outlet]
		private UINavigationController _navigationController
		{
			get; set;
		}
		
		[Outlet]
		private UITableViewController _commentsController
		{
			get; set;
		}
		
		public override int RowsInSection (UITableView tableview, int section)
		{
			return _details == null || _details.Comments == null ? 0 : _details.Comments.Length;
		}
		
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			var comment = _details.Comments[indexPath.Row];
			var cell = tableView.DequeueReusableCell("CommentCell");
			CommentCellController controller = null;
			if (cell == null)
			{
				controller = new CommentCellController();
				cell = controller.Load(comment);
				cell.Tag = _id++;
				_controllers[cell.Tag] = controller;
			}
			else
			{
				controller = _controllers[cell.Tag];
				controller.Load(comment);
			}
			
			return cell;
		}
		
		public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
		{
			var comment = _details.Comments[indexPath.Row];
			var size = tableView.StringSize(comment.Text, _font, _size, UILineBreakMode.WordWrap);
			return size.Height + 
				10 + //Margin 
				21; //Height of other label
		}
	}
}

