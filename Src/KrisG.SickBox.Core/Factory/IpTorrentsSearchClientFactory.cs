﻿using System;
using KrisG.IpTorrents.Client;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.Service;
using KrisG.Utility.Interfaces.Service;
using log4net;

namespace KrisG.SickBox.Core.Factory
{
    public class IpTorrentsSearchClientFactory : IIpTorrentsSearchClientFactory
    {
        private readonly IServerProvider _serverProvider;
        private readonly ILog _log;

        private readonly Lazy<ISearchClient> _lazyClient;

        public IpTorrentsSearchClientFactory(IServerProvider serverProvider, ILog log)
        {
            _lazyClient = new Lazy<ISearchClient>(BuildClient);

            _serverProvider = serverProvider;
            _log = log;
        }

        public ISearchClient GetClient()
        {
            return _lazyClient.Value;
        }

        private ISearchClient BuildClient()
        {
            var serverConfig = _serverProvider.Get<IIpTorrentsConfig>(ServerType.IpTorrents);

            string ipTorrentsUrl = serverConfig.Url;
            string ipTorrentsUsername = serverConfig.Username;
            string ipTorrentsPassword = serverConfig.Password;

            _log.DebugFormat("Creating IPTorrents Client [Url: {0}, Username: {1}, Password: {2}]",
                ipTorrentsUrl, ipTorrentsUsername, ipTorrentsPassword);

            return SearchClient.Create(ipTorrentsUrl, ipTorrentsUsername, ipTorrentsPassword);
        }
    }
}