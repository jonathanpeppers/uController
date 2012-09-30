using System;
using System.Threading;

namespace System.iPhone.UIKit
{
	public static class Execute
	{
		public static void Invoke(Action action)
		{
            action();
		}
		
		public static void BeginInvoke(Action action)
		{
            action();
		}
		
		public static void Background(Action action)
		{
            action();
		}
		
		public static void Background(Action backgroundAction, Action uiAction)
		{
            backgroundAction();
            uiAction();
		}
	}
}

