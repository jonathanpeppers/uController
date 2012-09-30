using System;
using System.Collections.Generic;
using System.IO;
using System.iPhone.UIKit;
using System.Net;
using System.Xml;
using System.Xml.Serialization;
namespace uController.API.Search
{
	#region Rss XmlSerializer classes

	[XmlRoot("rss")]
	public class Rss
	{
	    [XmlAttribute("version")]
	    public string Version
		{
			get;
			set;
		}
		
		[XmlElement("channel")]
	    public RssChannel Channel
		{
			get;
			set;
		}
	}
	 
	[XmlRoot("channel")]
	public class RssChannel
	{
		[XmlElement("title")]
	    public string Title
		{
			get;
			set;
		}
		
		[XmlElement("link")]
	    public string Link
		{
			get;
			set;
		}
		
		[XmlElement("description")]
	    public string Description
		{
			get;
			set;
		}

	    [XmlElement("item")]
	    public List<RssItem> Items
		{
			get;
			set;
		}
	}
	
	[XmlRoot("item")]
	public class RssItem
	{
		[XmlElement("title")]
	    public string Title
		{
			get;
			set;
		}

		[XmlElement("link")]
	    public string Link
		{
			get;
			set;
		}
		
		[XmlElement("guid")]
	    public string Guid
		{
			get;
			set;
		}
		
		[XmlElement("description")]
	    public string Description
		{
			get;
			set;
		}
		
		[XmlElement("enclosure")]
		public RssEnclosure Enclosure
		{
			get;
			set;
		}
	}
	
	[XmlRoot("enclosure")]
	public class RssEnclosure
	{
		[XmlAttribute("url")]
		public string Url
		{
			get;
			set;
		}
		
		[XmlAttribute("length")]
		public long Length
		{
			get;
			set;
		}
		
		[XmlAttribute("type")]
		public string Type
		{
			get;
			set;
		}
	}

	#endregion
	
	/// <summary>
	/// Base class for ISearchProviders that use Rss
	/// </summary>
	public abstract class RssProvider : ISearchProvider
	{
		private readonly XmlSerializer _serializer = null;
	
		public RssProvider ()
		{
			Timeout = 10000;
			
			_serializer = new XmlSerializer(typeof(Rss));	
		}

		#region ISearchProvider implementation

		public event EventHandler<SearchEventArgs> SearchCompleted = delegate { };
		
		public event EventHandler<ExtendedSearchEventArgs> ExtendedSearchCompleted = delegate { };

		public void Search (string searchText)
		{
			Execute.Background(() =>
			{
				try
				{
					HttpWebRequest request = HttpWebRequest.Create(BuildUrl(searchText)) as HttpWebRequest;
					request.Timeout = Timeout;
					request.Method = WebRequestMethods.Http.Get;
					request.UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
					using (HttpWebResponse response = request.GetResponse () as HttpWebResponse)
					{
						using (Stream stream = response.GetResponseStream ())
						{
							OnRssCompleted((Rss)_serializer.Deserialize(stream));
						}
					}
				}
				catch (Exception exc)
				{
					OnSearchCompleted(new SearchEventArgs(exc.Message));
				}
			});
		}
		
		public virtual void ExtendedSearch(TorrentUrl url)
		{ }
		
		public int Timeout
		{
			get;
			set;
		}

		#endregion

		protected abstract string BuildUrl(string searchText);

		protected abstract void OnRssCompleted(Rss rss);

		protected virtual void OnSearchCompleted(SearchEventArgs e)
		{
			SearchCompleted(this, e);
		}
		
		protected virtual void OnExtendedSearchCompleted(ExtendedSearchEventArgs e)
		{
			ExtendedSearchCompleted(this, e);	
		}
	}
}

