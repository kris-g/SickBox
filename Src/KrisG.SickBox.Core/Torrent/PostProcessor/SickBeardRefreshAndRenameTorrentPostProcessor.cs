using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.Utility.Interfaces;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Torrent.PostProcessor
{
    [ServiceImplementation("SickBeardRefreshAndRename")]
    public class SickBeardRefreshAndRenameTorrentPostProcessor : ITorrentPostProcessor
    {
        private readonly ISickBeardClientFactory _sickBeardClientFactory;
        private readonly IRetryAction _retryAction;
        private readonly ILog _log;

        public SickBeardRefreshAndRenameTorrentPostProcessor(
            ISickBeardClientFactory sickBeardClientFactory,
            IRetryAction retryAction,
            ILog log)
        {
            _sickBeardClientFactory = sickBeardClientFactory;
            _retryAction = retryAction;
            _log = log;
        }

        public void PostProcess(IEnumerable<TorrentDownloadResult> torrents)
        {
            var client = _sickBeardClientFactory.GetClient();
            var sickbeardRefreshJobs = torrents
                .Select(x => new { Id = x.Episode.ShowId, x.Episode.ShowName })
                .Distinct()
                .ToArray();

            const int numberOfRetries = 3;
            var delayBetweenRetries = TimeSpan.FromSeconds(5);

            foreach (var refreshJob in sickbeardRefreshJobs)
            {
                var job = refreshJob;

                try
                {
                    _retryAction.DoWithRetries(() => RefreshShow(job.Id, job.ShowName, client), numberOfRetries, delayBetweenRetries);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("SickBeard refresh failed: {0}", ex.Message);
                }

                try
                {
                    _retryAction.DoWithRetries(() => RenameFiles(job.Id, job.ShowName, client), numberOfRetries, delayBetweenRetries);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("SickBeard files rename failed: {0}", ex.Message);
                }
            }

            _log.InfoFormat("Sleeping for 20secs to allow SickBeard time to finish renames");
            Thread.Sleep(20000);            
        }

        private void RenameFiles(int id, string showName, ISickBeardClient client)
        {
            _log.InfoFormat("Triggering files rename in SickBeard [{0}]", JsonConvert.SerializeObject(new { Id = id, ShowName = showName }, Formatting.None));
            client.ShowFixFileNames(id);
        }

        private void RefreshShow(int id, string showName, ISickBeardClient client)
        {
            _log.InfoFormat("Triggering refresh in SickBeard [{0}]", JsonConvert.SerializeObject(new { Id = id, ShowName = showName }, Formatting.None));
            client.ShowRefresh(id);
            _log.InfoFormat("Sleeping for 10secs to allow SickBeard time to finish disk refresh");
            Thread.Sleep(10000);
        }
    }
}