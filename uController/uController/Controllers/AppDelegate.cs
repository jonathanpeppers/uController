using System;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.API;
using uController.API.Search;
using uController.Models;
namespace uController.Controllers
{
	[Register("AppDelegate")]
	public class AppDelegate : UIApplicationDelegate
	{
		private ITinyMessengerHub _hub = null;
				
		[Outlet]
		private UIWindow _window
		{
			get; set;
		}

		[Outlet]
		private UITabBarController _tabController
		{
			get; set;
		}
		
		[Outlet]
		private UIActivityIndicatorView _indicator
		{
			get; set;
		}
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{	
			//IoC setup
			var container = TinyIoCContainer.Current;
			container.Register<UIApplicationDelegate>(this);
			
			_hub = container.Resolve<ITinyMessengerHub>();
			
			var busy = container.Resolve<BusyIndicator>();
			busy.Indicator = _indicator;
			
			//Fire up window
			if (UIDevice.CurrentDevice.CheckSystemVersion (5, 0))
			{
				_window.RootViewController = _tabController;
			}
			else
			{
				_window.AddSubview (_tabController.View);
			}
			_window.BringSubviewToFront (_indicator);
			_window.MakeKeyAndVisible ();
			
			_hub.Publish(new ApplicationMessage(this, ApplicationMessageType.Loaded));
			return true;
		}

		public override void DidEnterBackground (UIApplication application)
		{
			_hub.Publish(new ApplicationMessage(this, ApplicationMessageType.Backgrounded));
		}

		public override void WillEnterForeground (UIApplication application)
		{
			_hub.Publish(new ApplicationMessage(this, ApplicationMessageType.Foregrounded));
		}
	}
}

