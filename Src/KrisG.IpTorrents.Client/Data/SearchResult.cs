using System;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;

namespace KrisG.IpTorrents.Client.Data
{
    [DebuggerDisplay("Name = {Name}")]
    public class SearchResult
    {
        public string Name { get; private set; }

        public FileSize FileSize { get; private set; }

        public int Snatches { get; private set; }

        public int Seeders { get; private set; }

        public int Leechers { get; private set; }

        public string Link { get; private set; }

        public string Type { get; private set; }

        public DateTime? PublishedDate { get; private set; }

        [JsonConstructor]
        public SearchResult(string name, FileSize fileSize, int snatches, int seeders, int leechers, string link, string type, DateTime? publishedDate)
        {
            Name = name;
            FileSize = fileSize;
            Snatches = snatches;
            Seeders = seeders;
            Leechers = leechers;
            Link = link;
            Type = type;
            PublishedDate = publishedDate;
        }

        public SearchResult(string name, FileSize fileSize, string link, string type, DateTime? publishedDate)
        {
            Name = name;
            FileSize = fileSize;
            Link = link;
            Type = type;
            PublishedDate = publishedDate;
        }
    }
}