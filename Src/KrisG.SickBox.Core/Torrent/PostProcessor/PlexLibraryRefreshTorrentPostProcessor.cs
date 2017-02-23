using System;
using System.Collections.Generic;
using KrisG.Utility.Interfaces;
using KrisG.Utility.Interfaces.Web;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Configuration.TorrentPostProcessor;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Service;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;

namespace KrisG.SickBox.Core.Torrent.PostProcessor
{
    [ServiceImplementation("PlexLibraryRefresh")]
    public class PlexLibraryRefreshTorrentPostProcessor : ITorrentPostProcessor, IConfigurable<IPlexLibraryRefreshTorrentPostProcessorConfig>
    {
        private readonly IServerProvider _serverProvider;
        private readonly IWebStreamProvider _webStreamProvider;
        private readonly IRetryAction _retryAction;
        private readonly ILog _log;

        public IPlexLibraryRefreshTorrentPostProcessorConfig Config { get; private set; }

        public PlexLibraryRefreshTorrentPostProcessor(
            IServerProvider serverProvider,
            IWebStreamProvider webStreamProvider,
            IRetryAction retryAction,
            ILog log)
        {
            _serverProvider = serverProvider;
            _webStreamProvider = webStreamProvider;
            _retryAction = retryAction;
            _log = log;
        }

        public void PostProcess(IEnumerable<TorrentDownloadResult> torrents)
        {
            var serverConfig = _serverProvider.Get<IUrlConfig>(ServerType.Plex);
            var plexRefreshTvUrl = string.Format(@"{0}/library/sections/{1}/refresh", serverConfig.Url, Config.TvLibraryId);
            
            _log.InfoFormat("Triggering refresh in Plex Media Server [Url: {0}]", plexRefreshTvUrl);

            const int numberOfAttempts = 3;
            var delayBetweenRetries = TimeSpan.FromSeconds(5);

            try
            {
                _retryAction.DoWithRetries(() => _webStreamProvider.GetStream(plexRefreshTvUrl), numberOfAttempts, delayBetweenRetries);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Plex Media Server refresh failed: {0}", ex.Message);
            }
        }
    }
}