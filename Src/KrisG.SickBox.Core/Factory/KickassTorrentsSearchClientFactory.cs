using System;
using KrisG.KickassTorrents.Client;
using KrisG.KickassTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Interfaces.Factory;
using log4net;

namespace KrisG.SickBox.Core.Factory
{
    public class KickassTorrentsSearchClientFactory : IKickassTorrentsSearchClientFactory
    {
        private readonly ILog _log;

        private readonly Lazy<ITorrentSearchClient> _lazyClient;

        public KickassTorrentsSearchClientFactory(ILog log)
        {
            _lazyClient = new Lazy<ITorrentSearchClient>(BuildClient);
            _log = log;
        }

        public ITorrentSearchClient GetClient()
        {
            return _lazyClient.Value;
        }

        private ITorrentSearchClient BuildClient()
        {
            _log.DebugFormat("Creating KickassTorrents Client [Url: {0}]", TorrentSearchClient.DefaultBaseUrl);

            return TorrentSearchClient.Create();
        }
    }
}