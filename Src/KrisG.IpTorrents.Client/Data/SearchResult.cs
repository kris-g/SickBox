using System.IO;

namespace KrisG.IpTorrents.Client.Data
{
    public class SearchResult
    {
        public string Name { get; private set; }

        public FileSize FileSize { get; private set; }

        public int Snatches { get; private set; }

        public int Seeders { get; private set; }

        public int Leechers { get; private set; }

        public string Link { get; private set; }

        public string Type { get; private set; }

        public SearchResult(string name, FileSize fileSize, int snatches, int seeders, int leechers, string link, string type)
        {
            Type = type;
            Link = link;
            Leechers = leechers;
            Seeders = seeders;
            Name = name;
            FileSize = fileSize;
            Snatches = snatches;
        }
    }
}