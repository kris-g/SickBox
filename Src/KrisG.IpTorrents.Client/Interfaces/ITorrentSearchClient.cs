using System.Collections.Generic;
using KrisG.IpTorrents.Client.Data;

namespace KrisG.IpTorrents.Client.Interfaces
{
    public interface ITorrentSearchClient
    {
        IEnumerable<SearchResult> Search(string query);

        void DownloadTorrent(SearchResult item, string downloadPath);
    }
}