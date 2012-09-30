using System;
namespace uController.API.Search
{
	/// <summary>
	/// A comment about a torrent
	/// </summary>
	public class Comment
	{
		/// <summary>
		/// User that wrote the comment
		/// </summary>
		public string User
		{
			get;
			set;
		}
		
		/// <summary>
		/// Extended info about the user: Reputation, etc.
		/// </summary>
		public string UserInfo
		{
			get;
			set;
		}
		
		/// <summary>
		/// Extended info about the comment: Votes, etc.
		/// </summary>
		public string CommentInfo
		{
			get;
			set;
		}
		
		/// <summary>
		/// The text of the comment
		/// </summary>
		public string Text
		{
			get;
			set;
		}
	}
}

