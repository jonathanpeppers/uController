using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace System.iPhone.UIKit
{
	public class BusyIndicator
	{
		public UIActivityIndicatorView Indicator
		{
			get;
			set;
		}
		
		public bool IsBusy
		{
			get { return Indicator == null ? false : !Indicator.Hidden; }
			set { if (Indicator != null) Indicator.Hidden = !value; }
		}
	}
}

