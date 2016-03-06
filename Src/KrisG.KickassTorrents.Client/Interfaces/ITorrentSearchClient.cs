using System.Collections.Generic;
using KrisG.KickassTorrents.Client.Data;

namespace KrisG.KickassTorrents.Client.Interfaces
{
    public interface ITorrentSearchClient
    {
        IEnumerable<SearchResult> Search(string query);

        void DownloadTorrent(SearchResult item, string downloadPath);
        void Initialise(string url);
    }
}