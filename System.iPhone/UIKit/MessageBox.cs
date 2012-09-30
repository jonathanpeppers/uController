using System;
using MonoTouch.UIKit;

namespace System.iPhone.UIKit
{
	[Flags]
	public enum MessageBoxButtons
	{
		OK = 0x1,
		Cancel = 0x2,
		Yes = 0x4,
		No = 0x8,
	}
	
	public static class MessageBox
	{
		public static void Show(string message)
		{
			Show(message, string.Empty, MessageBoxButtons.OK, null);
		}
		
		public static void Show(string message, string caption)
		{
			Show(message, caption, MessageBoxButtons.OK, null);
		}
		
		public static void Show(string message, string caption, MessageBoxButtons buttons)
		{
			Show(message, caption, buttons, null);
		}
		
		public static void Show(string message, string caption, MessageBoxButtons buttons, EventHandler<UIButtonEventArgs> handler)
		{
			using (UIAlertView alert = new UIAlertView())
			{
				alert.Message = message ?? string.Empty;
				alert.Title = caption;
				foreach (MessageBoxButtons button in Enum.GetValues(typeof(MessageBoxButtons))) 
				{
					if ((buttons & button) == button)
					{
						alert.AddButton(button.ToString());	
					}
				}
				if (handler != null)
				{
					alert.Dismissed += handler;
				}
				alert.Show();
			}
		}
		
		public static void Ask(string message, string caption, Action<bool> callback)
		{
			Show(message, caption, MessageBoxButtons.Yes | MessageBoxButtons.No, (sender, e) => callback(e.ButtonIndex == 0));
		}
		
		public static void AskToSave(Action<bool> callback)
		{
			Ask("Would you like to save your changes?", "Save?", callback);
		}
	}
}

