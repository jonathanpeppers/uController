using System;
using System.Linq;
using System.Text.RegularExpressions;
namespace uController.API.Search
{
	/// <summary>
	/// Class containing info about downloading a torrent
	/// </summary>
	public class TorrentUrl
	{
		private static readonly Regex _nameRegex = new Regex(@"\s*\[\d+/\d+\]$");
		
		/// <summary>
		/// Url of the .torrent file
		/// </summary>
		public string Url
		{
			get;
			set;
		}
		
		/// <summary>
		/// Url for extended info about the torrent
		/// </summary>
		public string InfoUrl
		{
			get;
			set;
		}
		
		/// <summary>
		/// Name of the torrent for the user
		/// </summary>
		public string Name
		{
			get;
			set;
		}
		
		public string NameTrimmed
		{
			get { return _nameRegex.Replace(Name, string.Empty); }
		}
		
		/// <summary>
		/// Category of the torrent
		/// </summary>
		public string Category
		{
			get;
			set;
		}
		
		public string CategoryTrimmed
		{
			get { return Category.Split(' ').Last().Trim(); }
		}
				
		
		/// <summary>
		/// Size of the torrent
		/// </summary>
		public string Size
		{
			get;
			set;
		}
		
		public string SizeTrimmed
		{
			get { return Size.Split(' ').Last().Trim(); }
		}
	}
}

