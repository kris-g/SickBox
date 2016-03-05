using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KrisG.SickBox.Core.Interfaces;
using KrisG.SickBox.Core.Interfaces.Data.SickBeard;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

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
            ILog log)
        {
            _wantedEpisodeProviders = wantedEpisodeProviders;
            _torrentSearchers = torrentSearchers;
            _torrentDownloadStart = torrentDownloadStart;
            _torrentCompleteNotifier = torrentCompleteNotifier;
            _downloadArchiver = downloadArchiver;
            _postProcessors = postProcessors;
            _log = log;
        }

        public void Execute()
        {
            try
            {
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
