using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
namespace uController
{
	[Register("CommentCellController")]
	public class CommentCellController : UIViewController
	{
		#region Constructors
	
		public CommentCellController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public CommentCellController (NSCoder coder) : base(coder)
		{
			Initialize();
		}
		
		public CommentCellController () : base("CommentCell", null)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			NSBundle.MainBundle.LoadNib("CommentCell", this, null);
		}
	
		#endregion
		
		[Outlet]
		private UILabel _comments
		{
			get; set;
		}
		
		[Outlet]
		private UILabel _user
		{
			get; set;
		}
		
		[Outlet]
		private UILabel _userInfo
		{
			get; set;
		}
	
		[Outlet]
		private UITableViewCell _cell
		{
			get; set;
		}
		
		public UITableViewCell Load(Comment comment)
		{
			_user.Text = comment.User;
			_userInfo.Text = comment.CommentInfo;
			_comments.Text = comment.Text;
			return _cell;
		}
	}
	
}

