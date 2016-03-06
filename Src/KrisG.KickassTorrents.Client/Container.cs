using System;
using KrisG.KickassTorrents.Client.Interfaces;
using KrisG.KickassTorrents.Client.Interfaces.Internal;
using KrisG.KickassTorrents.Client.Internal;
using KrisG.Utility.Interfaces.Web;
using KrisG.Utility.Web;
using Microsoft.Practices.Unity;

namespace KrisG.KickassTorrents.Client
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
                .RegisterType<ITorrentSearchClient>(new InjectionFactory(x => new TorrentSearchClient(x.Resolve<ISearchResultsParser>(), x.Resolve<IWebStreamProvider>())))
                .RegisterType<ISearchResultsParser, SearchResultsParser>()
                .RegisterType<IWebStreamProvider, WebStreamProvider>();

            return container;
        }
    }
}