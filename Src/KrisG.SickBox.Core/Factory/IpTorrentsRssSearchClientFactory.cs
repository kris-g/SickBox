using System;
using KrisG.IpTorrents.Client;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.Service;
using log4net;

namespace KrisG.SickBox.Core.Factory
{
    public class IpTorrentsRssSearchClientFactory : IIpTorrentsSearchClientFactory
    {
        private readonly IServerProvider _serverProvider;
        private readonly ILog _log;

        private readonly Lazy<ITorrentSearchClient> _lazyClient;

        public IpTorrentsRssSearchClientFactory(IServerProvider serverProvider, ILog log)
        {
            _lazyClient = new Lazy<ITorrentSearchClient>(BuildClient);

            _serverProvider = serverProvider;
            _log = log;
        }

        public ITorrentSearchClient GetClient()
        {
            return _lazyClient.Value;
        }

        private ITorrentSearchClient BuildClient()
        {
            var serverConfig = _serverProvider.Get<IIpTorrentsRssConfig>(ServerType.IpTorrentsRss);

            var ipTorrentsUrl = serverConfig.Url;

            _log.DebugFormat($"Creating IPTorrents RSS Client [Url: {ipTorrentsUrl}]");

            return TorrentRssSearchClient.Create(ipTorrentsUrl);
        }
    }
}