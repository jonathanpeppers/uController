using System;
namespace uController.API.Search
{
	public class SearchEventArgs : EventArgs
	{
		public SearchEventArgs (string error)
		{
			ErrorMessage = error;
		}

		public SearchEventArgs (TorrentUrl[] urls)
		{
			Urls = urls;
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

		public TorrentUrl[] Urls
		{
			get;
			private set;
		}
	}
}

