using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API.Search;
namespace uController.Controllers
{
	[Register("SearchResultCellController")]
	public class SearchResultCellController : UIViewController
	{
		#region Constructors
	
		public SearchResultCellController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public SearchResultCellController (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		public SearchResultCellController () : base("SearchResultCell", null)
		{
			Initialize();
		}

		protected virtual void Initialize()
		{
			NSBundle.MainBundle.LoadNib("SearchResultCell", this, null);
		}
	
		#endregion

		[Outlet]
		private UITableViewCell _cell
		{
			get; set;
		}

		[Outlet]
		private UILabel _title
		{
			get; set;
		}

		[Outlet]
		private UILabel _category
		{
			get; set;
		}
		
		[Outlet]
		private UILabel _size
		{
			get; set;
		}

		public UITableViewCell Load(TorrentUrl url)
		{
			_title.Text = url.Name;
			_category.Text = url.Category;
			_size.Text = url.Size;
			return _cell;
		}
	}
	
}

