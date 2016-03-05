using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.SickBeard.Client.Enums;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.SickBox.Core.Configuration.WantedEpisodeProvider;
using KrisG.SickBox.Core.Interfaces.Data.SickBeard;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces;
using KrisG.Utility.Interfaces.Configuration;
using log4net;

namespace KrisG.SickBox.Core.WantedEpisodeProvider
{
    [ServiceImplementation("SnatchedButStillMissingEpisode")]
    public class SnatchedButStillMissingEpisodeProvider : IWantedEpisodeProvider, IConfigurable<ISnatchedButStillMissingEpisodeProviderConfig>
    {
        private readonly ISickBeardClient _sickBeardClient;
        private readonly IRetryAction _retryAction;
        private readonly ILog _log;

        public ISnatchedButStillMissingEpisodeProviderConfig Config { get; private set; }

        public SnatchedButStillMissingEpisodeProvider(
            ISickBeardClientFactory clientFactory,
            IRetryAction retryAction,
            ILog log)
        {
            _sickBeardClient = clientFactory.GetClient();
            _retryAction = retryAction;
            _log = log;
        }

        public IEnumerable<IEpisode> FetchEpisodes()
        {
            const int numberOfAttempts = 3;
            var delayBetweenRetries = TimeSpan.FromSeconds(5);

            var historyEntries = _retryAction.DoWithRetries(() => _sickBeardClient.History(Config.HistoryItems), numberOfAttempts, delayBetweenRetries);
            var snatchedHistory = historyEntries
                .Where(x => x.Status == EpisodeStatus.Snatched)
                // group by show id and ep number to remove duplicate work
                .ToLookup(x => new { Id = x.ShowId, x.EpisodeNumber })
                .Select(x => x.First())
                .ToArray();

            _log.InfoFormat("SickBeard history search of last {0} items returned {1} 'Snatched' items", Config.HistoryItems, snatchedHistory.Length);

            foreach (var entry in snatchedHistory)
            {
                var entryCopy = entry;

                var seasonSummary = _retryAction.DoWithRetries(() => _sickBeardClient.ShowSeason(entryCopy.ShowId, entryCopy.SeasonNumber), numberOfAttempts, delayBetweenRetries);
                var matchingEpisode = seasonSummary.Episodes.FirstOrDefault(x => x.EpisodeNumber == entry.EpisodeNumber);

                if (matchingEpisode != null && matchingEpisode.Status == EpisodeStatus.Snatched)
                {
                    _log.InfoFormat("SickBeard show season search returned a matching 'Snatched' item [ShowName: {0}, ShowId: {1}, Season: {2}, Episode: {3}]",
                        entry.ShowName, entry.ShowId, entry.SeasonNumber, entry.EpisodeNumber);

                    yield return new HistoryEntryToEpisodeAdapter(entry);
                }
                else if (matchingEpisode != null)
                {
                    _log.DebugFormat("SickBeard episode was 'Snatched' in history but has status '{0}' in show details [ShowName: {1}, ShowId: {2}, Season: {3}, Episode: {4}]",
                        matchingEpisode.Status, entry.ShowName, entry.ShowId, entry.SeasonNumber, entry.EpisodeNumber);
                }
                else
                {
                    _log.DebugFormat("SickBeard episode was 'Snatched' in history but not present in show details [ShowName: {0}, ShowId: {1}, Season: {2}, Episode: {3}]",
                        entry.ShowName, entry.ShowId, entry.SeasonNumber, entry.EpisodeNumber);
                }
            }
        }
    }
}