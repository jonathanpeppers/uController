using System;
using System.Collections.Generic;
using System.IO;
using System.iPhone.UIKit;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using HtmlAgilityPack;

namespace uController.API.Search
{
    public class PirateBayProvider : ISearchProvider
    {
        private const string BaseUrl = "http://thepiratebay.se/";
        private const string Url = BaseUrl + "search/{0}/0/7/0";
        private const string UserAgent = "Mozilla/5.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";

        public PirateBayProvider()
        {
            Timeout = 30000;
        }

        public int Timeout
        {
            get;
            set;
        }

        public event EventHandler<SearchEventArgs> SearchCompleted = delegate { };

        public event EventHandler<ExtendedSearchEventArgs> ExtendedSearchCompleted = delegate { };

        public void Search(string searchText)
        {
            Execute.Background(() =>
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(string.Format(Url, HttpUtility.UrlEncode(searchText).Replace("%20", "+"))) as HttpWebRequest;
                    request.Timeout = Timeout;
                    request.Method = WebRequestMethods.Http.Get;
                    request.UserAgent = UserAgent;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            var html = new HtmlDocument();
                            html.Load(stream);

                            var baseUrl = new Uri(BaseUrl);
                            var urls = new List<TorrentUrl>();
                            var links = html.DocumentNode.SelectNodes("//div[@class='detName']/a");
                            var categories = html.DocumentNode.SelectNodes("//td[@class='vertTh']/center/a[2]");
                            var descriptions = html.DocumentNode.SelectNodes("//font[@class='detDesc']");

                            HtmlNode link;
                            Uri infoUrl;
                            string[] sizeSplit;
                            string size;
                            for (int i = 0; i < links.Count; i++)
                            {
                                link = links[i];
                                infoUrl = new Uri(baseUrl, link.Attributes["href"].Value);
                                sizeSplit = descriptions[i].InnerText.Split(',');
                                if (sizeSplit.Length > 1)
                                {
                                    size = HttpUtility.HtmlDecode(sizeSplit[1]);
                                    sizeSplit = size.Split(' ');
                                    size = sizeSplit.Last().Replace("iB", "B");
                                }
                                else
                                {
                                    size = null;
                                }

                                urls.Add(new TorrentUrl
                                {
                                    Name = HttpUtility.HtmlDecode(link.Attributes["title"].Value.Replace ("Details for", string.Empty)).Trim (),
                                    InfoUrl = infoUrl.ToString(),
                                    Url = BaseUrl + link.ParentNode.NextSibling.NextSibling.Attributes["href"].Value,
                                    Category = categories[i].InnerText.Trim (),
                                    Size = size.Trim (),
                                });
                            }

                            OnSearchCompleted(new SearchEventArgs(urls.ToArray()));
                        }
                    }
                }
                catch (Exception exc)
                {
                    OnSearchCompleted(new SearchEventArgs(exc.Message));
                }
            });
        }

        public void ExtendedSearch(TorrentUrl url)
        {
            Execute.Background(() =>
            {
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(url.InfoUrl) as HttpWebRequest;
                    request.Timeout = Timeout;
                    request.Method = WebRequestMethods.Http.Get;
                    request.UserAgent = UserAgent;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        using (Stream stream = response.GetResponseStream())
                        {
                            var html = new HtmlDocument();
                            html.Load(stream);

                            var uploaded = html.DocumentNode.SelectSingleNode("//dt[starts-with(text(), 'Uploaded')]");
                            var by = html.DocumentNode.SelectSingleNode("//dt[starts-with(text(), 'By')]");
                            var seeders = html.DocumentNode.SelectSingleNode("//dt[starts-with(text(), 'Seeders')]");
                            var leechers = html.DocumentNode.SelectSingleNode("//dt[starts-with(text(), 'Leechers')]");
                            var download = html.DocumentNode.SelectSingleNode("//div[@class='download']/a");

                            var details = new TorrentUrlDetails(url);
                            details.TorrentUrl.Url = download.Attributes["href"].Value;
                            details.CreatedOn = uploaded.NextSibling.NextSibling.InnerText.ToDate();
                            details.CreatedBy = HttpUtility.HtmlDecode(by.NextSibling.NextSibling.InnerText).Trim ();
                            details.Seeds = seeders.NextSibling.NextSibling.InnerText.ToInt();
                            details.Peers = leechers.NextSibling.NextSibling.InnerText.ToInt();

                            var comments = html.DocumentNode.SelectNodes("//div[starts-with(@id, 'comment-')]");
                            if (comments != null)
                            {
                                var commentsList = new List<Comment>(comments.Count);

                                foreach (var comment in comments)
                                {
                                    var byLine = comment.FirstChild.ChildNodes[1];

                                    commentsList.Add(new Comment
                                    {
                                        User = HttpUtility.HtmlDecode(byLine.InnerText).Trim(),
                                        CommentInfo = HttpUtility.HtmlDecode(byLine.NextSibling.InnerText).Trim(),
                                        Text = HttpUtility.HtmlDecode(comment.ChildNodes[1].InnerText).Trim(),
                                    });
                                }

                                details.Comments = commentsList.ToArray();
                            }
                            details.Files = new TorrentFile[0];
                            OnExtendedSearchCompleted(new ExtendedSearchEventArgs(details));
                        }
                    }
                }
                catch (Exception exc)
                {
                    OnExtendedSearchCompleted(new ExtendedSearchEventArgs(exc.Message));
                }
            });
        }

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
