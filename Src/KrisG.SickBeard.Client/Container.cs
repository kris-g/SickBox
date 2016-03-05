using System;
using System.Collections.Generic;
using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.SickBeard.Client.Parsers;
using KrisG.Utility.Interfaces.Web;
using KrisG.Utility.Web;
using Microsoft.Practices.Unity;

namespace KrisG.SickBeard.Client
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
                .RegisterType<ISickBeardClient>(new InjectionFactory(x => new SickBeardClient(x.Resolve<IWebStreamProvider>(), x.Resolve<IParserProvider>())))
                .RegisterType<IWebStreamProvider, WebStreamProvider>()
                .RegisterType<IParserProvider, ParserProvider>()
                .RegisterType<IJsonDataParser<IEnumerable<EpisodeSummary>>, EpisodeSummariesParser>()
                .RegisterType<IJsonDataParser<IEnumerable<Show>>, ShowsParser>()
                .RegisterType<IJsonDataParser<IEnumerable<int>>, SeasonListParser>()
                .RegisterType<IJsonDataParser<ShowDetails>, ShowDetailsParser>()
                .RegisterType<IJsonDataParser<IEnumerable<HistoryEntry>>, HistoryParser>()
                .RegisterType<IJsonDataParser<ShowRefreshResponse>, ShowRefreshResponseParser>();

            return container;
        }
    }
}