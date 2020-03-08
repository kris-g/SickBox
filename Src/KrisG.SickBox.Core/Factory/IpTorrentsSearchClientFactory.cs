using KrisG.IpTorrents.Client;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.Service;
using log4net;
using System;

namespace KrisG.SickBox.Core.Factory
{
    public class IpTorrentsSearchClientFactory : IIpTorrentsSearchClientFactory
    {
        private readonly IServerProvider _serverProvider;
        private readonly ILog _log;

        private readonly Lazy<ITorrentSearchClient> _lazyClient;

        public IpTorrentsSearchClientFactory(IServerProvider serverProvider, ILog log)
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
            var serverConfig = _serverProvider.Get<IIpTorrentsConfig>(ServerType.IpTorrents);

            var ipTorrentsUrl = serverConfig.Url;
            var ipTorrentsUsername = serverConfig.Username;
            var ipTorrentsPassword = serverConfig.Password;
            var ipTorrentsProxyConfig = serverConfig.Proxy;
            
            _log.DebugFormat("Creating IPTorrents Client [Url: {0}, Username: {1}, Password: {2}]",
                ipTorrentsUrl, ipTorrentsUsername, ipTorrentsPassword);

            throw new Exception();
            //return TorrentSearchClient.Create(ipTorrentsUrl, ipTorrentsUsername, ipTorrentsPassword, ipTorrentsProxyConfig);
        }
    }
}