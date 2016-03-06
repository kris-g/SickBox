using System.IO;

namespace KrisG.KickassTorrents.Client.Data
{
    public class SearchResult
    {
        public string Name { get; set; }

        public FileSize FileSize { get; private set; }

        public int Seeders { get; private set; }

        public int Peers { get; private set; }

        public string FileName { get; private set; }

        public string Link { get; private set; }

        public string Category { get; set; }

        public SearchResult(string name, FileSize fileSize, int seeders, int peers, string fileName, string link, string category)
        {
            Name = name;
            FileSize = fileSize;
            Seeders = seeders;
            Peers = peers;
            FileName = fileName;
            Link = link;
            Category = category;
        }
    }
}