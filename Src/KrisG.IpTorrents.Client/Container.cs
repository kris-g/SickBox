using System;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.Utility.Interfaces.Web;
using KrisG.Utility.Web;
using Microsoft.Practices.Unity;

namespace KrisG.IpTorrents.Client
{
    internal class Container// : IDisposable
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
                .RegisterType<ISearchClient>(new InjectionFactory(x => new SearchClient(x.Resolve<ISearchResultsParser>(), x.Resolve<IFormAuthenticatedWebStreamProvider>())))
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