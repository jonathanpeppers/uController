using System;
using System.Linq;
using System.Net;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
namespace System.iPhone.UIKit
{
	public enum DeviceType
	{
		iPhone,
		iPad,
		iPod,
	}
	
	public abstract class SettingsBase
	{
		public event EventHandler Synchronized = delegate { };
		
		public SettingsBase ()
		{
			UIDevice device = UIDevice.CurrentDevice;
			
			//Calculate Device and IsIPad
			string model = device.Model;
			if (string.IsNullOrEmpty(model))
			{
				Device = DeviceType.iPhone;
			}
			else if (model.Contains("iPad"))
			{
				Device = DeviceType.iPad;
			}
			else if (model.Contains("iPod"))
			{
				Device = DeviceType.iPod;
			}
			else
			{
				Device = DeviceType.iPhone;
			}
			IsMultitaskingSupported = device.IsMultitaskingSupported;
			
			//Load defaults
			Defaults = NSUserDefaults.StandardUserDefaults;
			Defaults.Init();
			Defaults.LoadDefaults();
		}
		
		public DeviceType Device
		{
			get;
			private set;
		}
		
		public bool IsIPad	
		{
			get { return Device == DeviceType.iPad; }
		}
		
		public bool IsMultitaskingSupported
		{
			get;
			private set;
		}
		
		protected virtual NSUserDefaults Defaults
		{
			get;
			set;
		}		
		
		public virtual void Synchronize()
		{ 
			Defaults.Synchronize();
		
			Synchronized(this, EventArgs.Empty);	
		}
	}
}

