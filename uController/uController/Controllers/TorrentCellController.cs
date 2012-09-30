using System;
using System.iPhone.IoC;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API;
namespace uController.Controllers
{
	[Register("TorrentCellController")]
	public class TorrentCellController : UIViewController
	{
		#region Constructors
	
		public TorrentCellController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public TorrentCellController (NSCoder coder) : base(coder)
		{
			Initialize();
		}

		public TorrentCellController () : base("TorrentCell", null)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			NSBundle.MainBundle.LoadNib("TorrentCell", this, null);
		}
	
		#endregion
	
		[Outlet]
		private UILabel _title
		{
			get; set;
		}

		[Outlet]
		private UITableViewCell _cell
		{
			get; set;
		}

		[Outlet]
		private UIProgressView _progress
		{
			get; set;
		}

		[Outlet]
		private UILabel _status
		{
			get; set;
		}

		[Outlet]
		private UILabel _eta
		{
			get; set;
		}

		[Outlet]
		private UILabel _up
		{
			get; set;
		}

		[Outlet]
		private UILabel _down
		{
			get; set;
		}

		public UITableViewCell Load(Torrent torrent)
		{
			_title.Text = torrent.Name;
			_status.Text = torrent.StatusString;
			_progress.Progress = (float)torrent.PercentProgress / 1000f;
			_eta.Text = torrent.EtaString;
			_up.Text = torrent.UpBps;
			_down.Text = torrent.DownBps;
			return _cell;
		}
	}
	
}

