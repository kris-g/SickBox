using System;
using System.Collections.Generic;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.Utility;
using KrisG.Utility.Interfaces;
using KrisG.Utility.Interfaces.Web;
using KrisG.Utility.Web;
using KrisG.SickBox.Core.Factory;
using KrisG.SickBox.Core.FileSystem;
using KrisG.SickBox.Core.Interfaces;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.PathProvider;
using KrisG.SickBox.Core.Interfaces.Service;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.SickBox.Core.Service;
using KrisG.SickBox.Core.SickBeard;
using KrisG.Utility.Interfaces.Configuration;
using KrisG.Utility.Interfaces.Service;
using KrisG.Utility.Service;
using log4net;
using Microsoft.Practices.Unity;
using UnityLog4NetExtension.Log4Net;
using IServiceProvider = KrisG.Utility.Interfaces.Service.IServiceProvider;
using IIpTorrentsTorrentSearchClient = KrisG.IpTorrents.Client.Interfaces.ITorrentSearchClient;
using IKickassTorrentsTorrentSearchClient = KrisG.KickassTorrents.Client.Interfaces.ITorrentSearchClient;

namespace KrisG.SickBox.Core
{
    public class Container
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
                .AddNewExtension<Log4NetExtension>()
                .RegisterType<IDownloader>(new ContainerControlledLifetimeManager(), new InjectionFactory(BuildDownloader))
                .RegisterType<ISickBeardClientFactory, SickBeardClientFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<ISickBeardClient>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<ISickBeardClientFactory>().GetClient()))
                .RegisterType<IEnumerable<IWantedEpisodeProvider>>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().GetAll<IWantedEpisodeProvider>()))
                .RegisterType<IEnumerable<ITorrentSearcher>>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().GetAll<ITorrentSearcher>()))
                .RegisterType<IIpTorrentsSearchClientFactory, IpTorrentsRssSearchClientFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<ITorrentSearchClient>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IIpTorrentsSearchClientFactory>().GetClient()))
                .RegisterType<IKickassTorrentsSearchClientFactory, KickassTorrentsSearchClientFactory>(new ContainerControlledLifetimeManager())
                .RegisterType<IKickassTorrentsTorrentSearchClient>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IKickassTorrentsSearchClientFactory>().GetClient()))
                .RegisterType<IShowNameProvider>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().Get<IShowNameProvider>(true) ?? x.Resolve<ShowNameProvider>()))
                .RegisterType<IEpisodeMatcher, RegexEpisodeMatcher>(new ContainerControlledLifetimeManager())
                .RegisterType<ITorrentDownloadStart>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().Get<ITorrentDownloadStart>()))
                .RegisterType<ITorrentCompleteNotifier>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().Get<ITorrentCompleteNotifier>()))
                .RegisterType<ITorrentDownloadArchiver>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().Get<ITorrentDownloadArchiver>(true)))
                .RegisterType<IArchivePathProvider>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().Get<IArchivePathProvider>()))
                .RegisterType<IStreamCopier, LoggedStreamCopier>(new ContainerControlledLifetimeManager())
                .RegisterType<IEnumerable<ITorrentPostProcessor>>(new ContainerControlledLifetimeManager(), new InjectionFactory(x => x.Resolve<IServiceProvider>().GetAll<ITorrentPostProcessor>(true)))
                .RegisterType<IWebStreamProvider, WebStreamProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IConfigurationReader, CommandLineConfigurationFileReader>(new ContainerControlledLifetimeManager())
                .RegisterType<IServiceProvider, ConfigurationServiceProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IServiceResolver, ServiceResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<IConfigResolver, CustomConfigResolver>(new ContainerControlledLifetimeManager())
                .RegisterType<IServerProvider, ConfigurationServerProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IFileSystemProvider, FileSystemProvider>(new ContainerControlledLifetimeManager())
                .RegisterType<IRetryAction, RetryAction>(new ContainerControlledLifetimeManager())
                ;

            return container;
        }

        private static DefaultDownloader BuildDownloader(IUnityContainer container)
        {
            var logger = LogManager.GetLogger(typeof(DefaultDownloader));
            try
            {
                return new DefaultDownloader(
                    container.Resolve<IEnumerable<IWantedEpisodeProvider>>(),
                    container.Resolve<IEnumerable<ITorrentSearcher>>(),
                    container.Resolve<ITorrentDownloadStart>(),
                    container.Resolve<ITorrentCompleteNotifier>(),
                    container.Resolve<ITorrentDownloadArchiver>(),
                    container.Resolve<IEnumerable<ITorrentPostProcessor>>(),
                    logger);
            }
            catch (Exception ex)
            {
                logger.Fatal("Failed to build downloader", ex);
                throw;
            }
        }
    }
}