using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.IpTorrents.Client;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Interfaces;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Service;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;

namespace KrisG.SickBox.Core
{
    public class DefaultDownloader : IDownloader
    {
        private readonly IEnumerable<IWantedEpisodeProvider> _wantedEpisodeProviders;
        private readonly IEnumerable<ITorrentSearcher> _torrentSearchers;
        private readonly ITorrentDownloadStart _torrentDownloadStart;
        private readonly ITorrentCompleteNotifier _torrentCompleteNotifier;
        private readonly ITorrentDownloadArchiver _downloadArchiver;
        private readonly IEnumerable<ITorrentPostProcessor> _postProcessors;
        private readonly IServerProvider _serverProvider;
        private readonly ILog _log;

        public static IDownloader Create()
        {
            return new Container().Resolve<IDownloader>();
        }

        internal DefaultDownloader(
            IEnumerable<IWantedEpisodeProvider> wantedEpisodeProviders,
            IEnumerable<ITorrentSearcher> torrentSearchers,
            ITorrentDownloadStart torrentDownloadStart,
            ITorrentCompleteNotifier torrentCompleteNotifier,
            ITorrentDownloadArchiver downloadArchiver,
            IEnumerable<ITorrentPostProcessor> postProcessors,
            IServerProvider serverProvider,
            ILog log)
        {
            _wantedEpisodeProviders = wantedEpisodeProviders;
            _torrentSearchers = torrentSearchers;
            _torrentDownloadStart = torrentDownloadStart;
            _torrentCompleteNotifier = torrentCompleteNotifier;
            _downloadArchiver = downloadArchiver;
            _postProcessors = postProcessors;
            _serverProvider = serverProvider;
            _log = log;
        }

        public void Execute()
        {
            try
            {
                // hack: trying to trigger cache load/appendn on each execute
                var rssConfig = _serverProvider.Get<IIpTorrentsRssConfig>(ServerType.IpTorrentsRss);
                TorrentRssSearchClient.Create(rssConfig.Url);

                var wantedEpisodes = _wantedEpisodeProviders
                    .SelectMany(x => x.FetchEpisodes())
                    // take an arbitrary distinct set of items, in case different services return dupes
                    .ToLookup(x => new {x.ShowId, x.SeasonNumber, x.EpisodeNumber})
                    .Select(x => x.First())
                    .ToList();

                if (wantedEpisodes.Count == 0)
                {
                    _log.InfoFormat("Found no episodes wanted for download");
                    return;
                }

                var torrentSearchersArray = _torrentSearchers.ToArray();
                var fetchedTorrentFiles = wantedEpisodes
                    // take the first searcher result who succeeds
                    .Select(ep => torrentSearchersArray.Select(s => s.Search(ep)).FirstOrDefault(res => res != null))
                    .Where(searchResult => searchResult != null)
                    .ToList();

                if (fetchedTorrentFiles.Count == 0)
                {
                    _log.InfoFormat("No torrents downloaded");
                    return;
                }

                _torrentDownloadStart.StartDownload(fetchedTorrentFiles.ToArray());

                var completeTorrents = _torrentCompleteNotifier.WaitForAll(fetchedTorrentFiles).ToArray();

                if (completeTorrents.Length == 0)
                {
                    _log.InfoFormat("No torrents completed ready for FTP downloads");
                    return;
                }

                if (_downloadArchiver != null)
                {
                    _downloadArchiver.Finalise(completeTorrents);                    
                }
                else
                {
                    _log.InfoFormat("No {0} service in configuration, skipping", ServiceInterfaceAttribute.GetName<ITorrentDownloadArchiver>());
                }

                foreach (var postProcessor in _postProcessors)
                {
                    postProcessor.PostProcess(completeTorrents);
                }
            }
            catch (Exception ex)
            {
                _log.Fatal("Fatal error occurred", ex);
                throw;
            }
        }
    }
}
