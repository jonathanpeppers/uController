using System;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.Models;
namespace uController.Controllers
{
	[Register("TabController")]
	public class TabController : UITabBarController
	{
		private ITinyMessengerHub _hub = null;
		
		#region Constructors
	
		public TabController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public TabController (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			var container = TinyIoCContainer.Current;
			_hub = container.Resolve<ITinyMessengerHub>();
			
			_hub.Subscribe<EmptyUrlMessage>(m =>
			{
				SelectedIndex = 2;	
				MessageBox.Show("Please enter a URL for your uTorrent server.", "No URL Found");
			});
			
			_hub.Subscribe<AddTorrentMessage>(_ => SelectedIndex = 0);
		}
	
		#endregion
	}
	
}

