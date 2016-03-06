﻿using System;
using System.IO;
using System.Linq;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.TorrentSearcher;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Torrent.Searcher
{
    [ServiceImplementation("IpTorrents")]
    public class IpTorrentsSearcher : ITorrentSearcher, IConfigurable<ITorrentSearcherConfig>
    {
        private readonly IShowNameProvider _showNameProvider;
        private readonly IEpisodeMatcher _episodeMatcher;
        private readonly ILog _log;
        private readonly ITorrentSearchClient _torrentSearchClient;
        
        public ITorrentSearcherConfig Config { get; private set; }

        public IpTorrentsSearcher(
            IIpTorrentsSearchClientFactory searchClientFactory,
            IShowNameProvider showNameProvider,
            IEpisodeMatcher episodeMatcher,
            ILog log)
        {
            _showNameProvider = showNameProvider;
            _episodeMatcher = episodeMatcher;
            _log = log;

            _torrentSearchClient = searchClientFactory.GetClient();
        }

        public TorrentSearchResult Search(IEpisode episode)
        {
            var cleanedShowName = _showNameProvider.Get(episode);

            var query = string.Format("{0} s{1:00}e{2:00}", cleanedShowName, episode.SeasonNumber, episode.EpisodeNumber);
            if (Config.SearchQueryAdditions.Length != 0)
            {
                query = string.Format("{0} {1}", query, Config.SearchQueryAdditions.JoinStrings(" "));
            }

            var results = _torrentSearchClient.Search(query).ToArray();
            if (Config.CategoriesToExclude.Any())
            {
                var countBeforeFilter = results.Length;
                results = results.Where(x => !Config.CategoriesToExclude.Contains(x.Type)).ToArray();
                _log.InfoFormat("IPTorrents search query '{0}' returned {1} items, {2} remaining after type filter [{3}]",
                    query, countBeforeFilter, results.Length, Config.CategoriesToExclude.JoinStrings("|"));
            }
            else
            {
                _log.InfoFormat("IPTorrents search query '{0}' returned {1} items", query, results.Length);
            }

            results = results.Where(x => _episodeMatcher.IsMatch(x.Name, episode)).ToArray();

            if (results.Length != 0)
            {
                var downloadItem = results.OrderByDescending(x => x.Snatches).First();

                _log.InfoFormat("Downloading best candidate [{0}]", JsonConvert.SerializeObject(downloadItem, Formatting.None));

                var downloadPath = BuildDownloadPath(downloadItem);
                _torrentSearchClient.DownloadTorrent(downloadItem, downloadPath);

                return new TorrentSearchResult(episode, downloadPath);
            }

            return null;
        }

        private string BuildDownloadPath(SearchResult item)
        {
            var fileName = item.Link.Substring(item.Link.LastIndexOf('/') + 1);

            var execFolder = AppDomain.CurrentDomain.BaseDirectory;
            var downloadFolder = Path.Combine(execFolder, "Torrents");
            var downloadPath = Path.Combine(downloadFolder, fileName);

            return downloadPath;
        }
    }
}