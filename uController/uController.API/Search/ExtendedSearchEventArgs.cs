using System;
namespace uController.API.Search
{
	public class ExtendedSearchEventArgs : EventArgs
	{
		public ExtendedSearchEventArgs (string error)
		{
			ErrorMessage = error;
		}
		
		public ExtendedSearchEventArgs (TorrentUrlDetails details)
		{
			TorrentDetails = details;
			Success = true;
		}
		
		public bool Success
		{
			get;
			private set;
		}

		public string ErrorMessage
		{
			get;
			private set;
		}
		
		public TorrentUrlDetails TorrentDetails
		{
			get;
			private set;
		}
	}
}

