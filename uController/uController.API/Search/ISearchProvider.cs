using System;
namespace uController.API.Search
{
	/// <summary>
	/// Interface describing a torrent search provider
	/// </summary>
	public interface ISearchProvider
	{
		/// <summary>
		/// Search has completed
		/// </summary>
		event EventHandler<SearchEventArgs> SearchCompleted;
		
		/// <summary>
		/// Extended search has completed
		/// </summary>
		event EventHandler<ExtendedSearchEventArgs> ExtendedSearchCompleted;
		
		/// <summary>
		/// Searches torrents based on the text from the user
		/// </summary>
		/// <param name="searchText">
		/// A <see cref="System.String"/>
		/// </param>
		void Search(string searchText);
		
		/// <summary>
		/// Searches extended info on the torrent
		/// </summary>
		/// <param name="url">
		/// A <see cref="TorrentUrl"/>
		/// </param>
		void ExtendedSearch(TorrentUrl url);
	}
}

