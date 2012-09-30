using System;
using System.Threading;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.iPhone.IoC;
namespace System.iPhone.UIKit
{
	public static class Execute
	{
		private static UIApplicationDelegate _app = TinyIoCContainer.Current.Resolve<UIApplicationDelegate>();
			
		public static void Invoke(NSAction action)
		{
			if (_app != null)
			{
				_app.InvokeOnMainThread(action);
			}
		}
		
		public static void BeginInvoke(NSAction action)
		{
			if (_app != null)
			{
				_app.BeginInvokeOnMainThread(action);
			}
		}
		
		public static void Background(NSAction action)
		{
			action.BeginInvoke(null, null);
		}
		
		public static void Background(NSAction backgroundAction, NSAction uiAction)
		{
			backgroundAction.BeginInvoke(r => BeginInvoke(uiAction), null);
		}
	}
}

