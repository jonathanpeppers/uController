using System;
using GoogleAnalytics;
using MonoTouch.Foundation;
namespace uController
{
	public class GoogleService
	{
		private const string _code = "UA-5513098-2";
		private const int _time = 10;
		private GANTracker _tracker = null;
		
		public void Start()
		{  
			try
			{
				_tracker = new GANTracker();
				_tracker.StartTracker (_code, _time, null);  
				                               
				TrackPage("/app_startup");
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.ToString());	
			}
		}
		
		public void TrackPage(string page)
		{
			try
			{
				if (_tracker != null)	
				{
					NSError error = null;
					if (!_tracker.TrackPageView (page, out error))  
					{
						Console.WriteLine("Error with Google Analytics:" + error.LocalizedDescription);	
					}
					
					_tracker.Dispatch ();  	
				}
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.ToString());	
			}
		}
	}
}

