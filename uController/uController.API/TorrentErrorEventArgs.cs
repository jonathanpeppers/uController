using System;
namespace uController.API
{
	public class TorrentErrorEventArgs : EventArgs
	{
		public TorrentErrorEventArgs (Exception exc)
		{
			Exception = exc;
		}
		
		public Exception Exception
		{
			get;
			private set; 
		}
	}
}

