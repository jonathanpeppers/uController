using System;
using System.IO;
using MonoTouch.Foundation;
namespace System.iPhone.UIKit
{
	public static class Paths
	{
		public static readonly string BaseDir,
			Documents,
			Preferences,
			BundlePath,
			SettingsPlist;
		
		static Paths()
		{
			BaseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..");
			Documents = Path.Combine(BaseDir, "Documents");
			Preferences = Path.Combine(BaseDir, "Library/Preferences");
			BundlePath = NSBundle.MainBundle.BundlePath;
			SettingsPlist = Path.Combine(BundlePath, "Settings.bundle/Root.plist");
		}
	}
}

