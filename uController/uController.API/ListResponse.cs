using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
namespace uController.API
{
	public class ListResponse
	{
		public ListResponse (JsonValue json)
		{
			if (json != null)
			{
				BuildNumber = json["build"];
				if (json.ContainsKey ("torrentc"))
				{
					CacheID = json["torrentc"];
				}
				if (json.ContainsKey("torrents"))
				{
					Torrents = ((JsonArray)json["torrents"])
						.Select(j => new Torrent(j))
						.ToDictionary(t => t.ID);
				}
				if (json.ContainsKey("torrentp"))
				{
					ChangedTorrents = ((JsonArray)json["torrentp"])
						.Select(j => new Torrent(j))
						.ToArray();
				}
				if (json.ContainsKey("torrentm"))
				{
					RemovedTorrents = ((JsonArray)json["torrentm"])
						.Select(j => new Torrent(j))
						.ToArray();
				}
			}
		}
		
		public int BuildNumber
		{
			get;
			set;
		}
		
		public string CacheID
		{
			get;
			set;
		}
		
		public Dictionary<string, Torrent> Torrents
		{
			get;
			set;
		}
		
		public Torrent[] ChangedTorrents
		{
			get;
			set;
		}
		
		public Torrent[] RemovedTorrents
		{
			get;
			set;
		}
	}
}

