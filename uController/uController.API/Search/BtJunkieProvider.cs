using System;
using System.Collections.Generic;
using System.IO;
using System.iPhone.UIKit;
using System.Web;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
namespace uController.API.Search
{
	public class BtJunkieProvider : RssProvider
	{
		private Regex _catRegex = new Regex("^Category: [^\\s]+", RegexOptions.IgnoreCase),
			_sizeRegex = new Regex("Size: .+$", RegexOptions.IgnoreCase),
			_peersRegex = new Regex("\\s*peers\\s*", RegexOptions.IgnoreCase),
			_seedsRegex = new Regex("\\s*seeds\\s*", RegexOptions.IgnoreCase),
			_byRegex = new Regex("\\s+by\\s+", RegexOptions.IgnoreCase),
			_fileSizeRegex = new Regex("\\[(\\d+.?(B|bytes))\\]$", RegexOptions.IgnoreCase);
		
		#region implemented abstract members of uController.API.Search.RssProvider

		protected override string BuildUrl (string searchText)
		{
			return "http://btjunkie.org/rss.xml?query=" + HttpUtility.UrlEncode(searchText).Replace("%20", "+") + "&o=52";
		}
				
		protected override void OnRssCompleted (Rss rss)
		{
			OnSearchCompleted(new SearchEventArgs(rss.Channel.Items
				.Skip(1) //Skip 1, the first is some ad-crap
				.Select(i => new TorrentUrl 
				{ 
					Url = i.Enclosure.Url,
					InfoUrl = i.Link,
					Name = i.Title, 
					Size = GetSize(i),
					Category = GetCategory(i),
				})
				.ToArray()));
		}

		#endregion
		
		public override void ExtendedSearch (TorrentUrl url)
		{
#if !NUnit
			Execute.Background(() =>
			{
#endif
				try
				{
					if (string.IsNullOrEmpty(url.InfoUrl))
					{
						throw new Exception("Could not find a URL to access this torrent's details.");	
					}
					
					HttpWebRequest request = HttpWebRequest.Create(url.InfoUrl) as HttpWebRequest;
					request.Timeout = Timeout;
					request.Method = WebRequestMethods.Http.Get;
					using (HttpWebResponse response = request.GetResponse () as HttpWebResponse)
					{
						using (Stream stream = response.GetResponseStream ())
						{
							TorrentUrlDetails details = new TorrentUrlDetails(url);
							
							HtmlDocument html = new HtmlDocument();
							html.Load(stream);
							
							var table = html.DocumentNode.SelectSingleNode("//th[@style='background-color: #EEEEEE;']/div/table");
							
							//Seeds and Peers
							string status = table.SelectSingleNode("//tr[3]/th[2]/font/b").InnerText;
							string[] split = status.Split('|');
							string seeds = _seedsRegex.Replace(split[0], string.Empty),
								peers = _peersRegex.Replace(split[1], string.Empty);
							
							details.Seeds = seeds.Replace(",", string.Empty).ToLong();
							details.Peers = peers.Replace(",", string.Empty).ToLong();
						
							//Date and user
							string torrentBy = table.SelectSingleNode("//tr[3]/th[4]/font/b").InnerText;
							split = _byRegex.Split(torrentBy);
							
							details.CreatedOn = split[0].ToDate();
							details.CreatedBy = HttpUtility.HtmlDecode(split[1]);
							
							//Comments
							var commentNodes = html.DocumentNode.SelectNodes("//div/table[@style='border-top: 1px solid #000000; border-left: 1px solid #000000; border-right: 1px solid #000000;']/tr/th");
							var comments = new List<Comment>();
							var comment = new Comment();
							for (int i = 1; i < commentNodes.Count; i++)
							{
								HtmlNode node = commentNodes[i];
								
								//User info, votes, etc.
								if (i % 2 == 1)
								{
									if (node.ChildNodes.Count < 6)
									{
										break;	
									}
									
									comment.User = HttpUtility.HtmlDecode(node.ChildNodes[1].InnerText).Trim();
									comment.UserInfo = HttpUtility.HtmlDecode(node.ChildNodes[3].InnerText).Trim();
									comment.CommentInfo = HttpUtility.HtmlDecode(node.ChildNodes[5].InnerText).Trim();
								}
								//Comment body
								else
								{
									comment.Text = HttpUtility.HtmlDecode(node.InnerText).Trim();
									comments.Add(comment);
									comment = new Comment();
								}
							}
							details.Comments = comments.ToArray();
							
							//Files
							details.Files = html.DocumentNode
								.SelectNodes("//table[@class='tor_details']/tr/th/font")
								.Select(n => new TorrentFile
									{
										Name = HttpUtility.HtmlDecode(_fileSizeRegex.Replace(n.InnerText, string.Empty)).Trim(),
										Size = HttpUtility.HtmlDecode(_fileSizeRegex.Match(n.InnerText).Groups[1].Value).Trim(),
									})
								.ToArray();
							
							OnExtendedSearchCompleted(new ExtendedSearchEventArgs(details));
						}
					}
				}
				catch (Exception exc)
				{
					OnExtendedSearchCompleted(new ExtendedSearchEventArgs(exc.Message));
				}
#if !NUnit
			});
#endif
		}
		
		private string GetCategory(RssItem rss)
		{
			Match match = _catRegex.Match(rss.Description);
			if (match.Success)
			{
				return match.Value;
			}
			return null;
		}
		
		private string GetSize(RssItem rss)
		{
			Match match = _sizeRegex.Match(rss.Description);
			if (match.Success)
			{
				return match.Value;
			}
			return null;
		}
	}
}

