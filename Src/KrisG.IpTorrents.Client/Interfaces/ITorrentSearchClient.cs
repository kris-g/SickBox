﻿using System.Collections.Generic;
using KrisG.IpTorrents.Client.Data;

namespace KrisG.IpTorrents.Client.Interfaces
{
    public interface ITorrentSearchClient
    {
        void Initialise(string url, string username, string password, IProxyConfig proxyConfig);

        IEnumerable<SearchResult> Search(string query);

        void DownloadTorrent(SearchResult item, string downloadPath);
    }
}