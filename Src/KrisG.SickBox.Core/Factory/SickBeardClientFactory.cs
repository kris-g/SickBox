using System;
using KrisG.SickBeard.Client;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.Server;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.Service;
using log4net;

namespace KrisG.SickBox.Core.Factory
{
    public class SickBeardClientFactory : ISickBeardClientFactory
    {
        private readonly IServerProvider _serverProvider;
        private readonly ILog _log;

        private readonly Lazy<ISickBeardClient> _lazyClient;

        public SickBeardClientFactory(IServerProvider serverProvider, ILog log)
        {
            _lazyClient = new Lazy<ISickBeardClient>(BuildSickBeardClient);

            _serverProvider = serverProvider;
            _log = log;
        }

        public ISickBeardClient GetClient()
        {
            return _lazyClient.Value;
        }

        private ISickBeardClient BuildSickBeardClient()
        {
            var serverConfig = _serverProvider.Get<ISickBeardClientConfig>(ServerType.SickBeard);

            _log.DebugFormat("Creating SickBeard Client [Url: {0}, ApiKey: {1}]", serverConfig.Url, serverConfig.ApiKey);

            return SickBeardClient.Create(serverConfig.Url, serverConfig.ApiKey);
        }
    }
}