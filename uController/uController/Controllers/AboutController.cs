using System;
using System.iPhone.IoC;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using uController.Models;
namespace uController.Controllers
{
	[Register("AboutController")]
	public class AboutController : UIViewController
	{
		private const string _paypalUrl = "https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=THKN4ZR7KXZXY";
		private ITinyMessengerHub _hub = null;
		
		#region Constructors
	
		public AboutController (IntPtr handle) : base (handle)
		{
			Initialize();
		}
	
		[Export("initWithCoder:")]
		public AboutController (NSCoder coder) : base(coder)
		{
			Initialize();
		}
	
		protected virtual void Initialize()
		{
			_hub = TinyIoCContainer.Current.Resolve<ITinyMessengerHub>();
			_hub.Subscribe<DonateMessage>(m => Donate());
		}
	
		#endregion
		
		[Outlet]
		private UILabel _label
		{
			get; set;
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			_label.Text = 
				
				"uController" + Environment.NewLine +
				"was thoughtfully created by" + Environment.NewLine +
				"Jonathan Peppers" + Environment.NewLine +
					
				Environment.NewLine +
					
				"If you enjoy the app, consider donating $.99 by clicking below.";
		}
		
		[Export("Donate:")]
		public void Donate()
		{
			using (NSUrl url = new NSUrl(_paypalUrl))
			{
				UIApplication.SharedApplication.OpenUrl(url);
			}
		}
	}
	
}

