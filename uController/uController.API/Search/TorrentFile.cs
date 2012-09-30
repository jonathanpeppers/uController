using System;
namespace uController.API.Search
{
	/// <summary>
	/// File in a torrent
	/// </summary>
	public class TorrentFile
	{
		/// <summary>
		/// Name of the file
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		
		/// <summary>
		/// Size of the file
		/// </summary>
		public string Size
		{
			get;
			set;
		}
	}
}

