using System;
using System.iPhone.IoC;
using System.iPhone.UIKit;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace uController.Models
{
	public class Settings : SettingsBase
	{
		private enum Keys
		{
			Url,
			Port,
			Username,
			Password,
			NumberOfRuns,
		}

		public Settings (ITinyMessengerHub hub)
		{
			hub.Subscribe<ApplicationMessage>(m =>
			{
				if (m.Content == ApplicationMessageType.Backgrounded || m.Content == ApplicationMessageType.Foregrounded)
				{
					Synchronize();
				}
			});
		}
		
		public string Url
		{
			get { return Defaults.StringForKey(Keys.Url.ToString()); }
			set { Defaults.SetString(value, Keys.Url.ToString()); }
		}
		
		public int Port
		{
			get { return Defaults.IntForKey(Keys.Port.ToString()); }
			set { Defaults.SetInt(value, Keys.Port.ToString()); }
		}
		
		public string Username
		{
			get { return Defaults.StringForKey(Keys.Username.ToString()); }
			set { Defaults.SetString(value, Keys.Username.ToString()); }
		}
		
		public string Password
		{
			get { return Defaults.StringForKey(Keys.Password.ToString()); }
			set { Defaults.SetString(value, Keys.Password.ToString()); }
		}
		
		public int NumberOfRuns
		{
			get { return Defaults.IntForKey(Keys.NumberOfRuns.ToString()); }
			set { Defaults.SetInt(value, Keys.NumberOfRuns.ToString()); }
		}
	}
}

