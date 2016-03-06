using System;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.IpTorrents.Client.Interfaces.Internal;
using KrisG.IpTorrents.Client.Internal;
using Microsoft.Practices.Unity;

namespace KrisG.IpTorrents.Client
{
    internal class Container
    {
        private static readonly Lazy<IUnityContainer> _container = new Lazy<IUnityContainer>(BuildContainer);

        public T Resolve<T>()
        {
            return _container.Value.Resolve<T>();
        }

        private static IUnityContainer BuildContainer()
        {
            var container = new UnityContainer();

            container
                .RegisterType<ITorrentSearchClient>(new InjectionFactory(x => new TorrentSearchClient(x.Resolve<ISearchResultsParser>(), x.Resolve<IFormAuthenticatedWebStreamProvider>())))
                .RegisterType<ISearchResultsParser, SearchResultsParser>()
                .RegisterType<IFormAuthenticatedWebStreamProvider, FormAuthenticatedWebStreamProvider>();

            return container;
        }

        public void Dispose()
        {
            if (_container.IsValueCreated)
            {
                _container.Value.Dispose();
            }
        }
    }
}