using System;
namespace uController.API.Search
{
	/// <summary>
	/// Extended details about a torrent URL
	/// </summary>
	public class TorrentUrlDetails
	{
		public TorrentUrlDetails (TorrentUrl torrent)
		{
			TorrentUrl = torrent;
		}
		
		/// <summary>
		/// The original TorrentUrl
		/// </summary>
		public TorrentUrl TorrentUrl
		{
			get;
			private set;
		}
		
		/// <summary>
		/// # of Seeds
		/// </summary>
		public long Seeds
		{
			get;
			set;
		}
		
		/// <summary>
		/// # of Peers
		/// </summary>
		public long Peers
		{
			get;
			set;
		}
		
		/// <summary>
		/// Date the torrent was created
		/// </summary>
		public DateTime CreatedOn
		{
			get;
			set;
		}
		
		/// <summary>
		/// User that created the torrent
		/// </summary>
		public string CreatedBy
		{
			get;
			set;
		}
		
		/// <summary>
		/// List of comments on the torrent
		/// </summary>
		public Comment[] Comments
		{
			get;
			set;
		}
		
		/// <summary>
		/// List of files included in the torrent
		/// </summary>
		public TorrentFile[] Files
		{
			get;
			set;
		}
	}
}

