using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using KrisG.SickBeard.Client.Data;
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
    [ServiceImplementation("RecentWanted")]
    public class RecentWantedEpisodeProvider : IWantedEpisodeProvider, IConfigurable<IRecentWantedEpisodeProviderConfig>
    {
        private const int NumberOfAttempts = 5;
        private readonly TimeSpan _delayBetweenRetries;

        private readonly IRetryAction _retryAction;
        private readonly ILog _log;
        private readonly ISickBeardClient _sickBeardClient;

        public IRecentWantedEpisodeProviderConfig Config { get; private set; }

        public RecentWantedEpisodeProvider(
            ISickBeardClientFactory clientFactory,
            IRetryAction retryAction,
            ILog log)
        {
            _delayBetweenRetries = TimeSpan.FromSeconds(10);
            _sickBeardClient = clientFactory.GetClient();            
            
            _retryAction = retryAction;
            _log = log;
        }

        public IEnumerable<IEpisode> FetchEpisodes()
        {
            var allShows = _retryAction.DoWithRetries(() => _sickBeardClient.Shows(), NumberOfAttempts, _delayBetweenRetries);
            var filteredShows = allShows.Where(x => !x.Paused).Where(x => x.Status == ShowStatus.Continuing).ToArray();

            _log.DebugFormat("Searching unpaused continuing SickBeard shows for 'Wanted' episodes aired in the last {0} days [{1}]",
                Config.WithinDays, string.Join(", ", filteredShows.Select(x => x.ShowName)));

            var recentWantedEpisodes = filteredShows
                .Select(x => new { Show = x, x.Id, MostRecentSeasons = GetMostRecentSeasonNumber(x.Id) })
                .SelectMany(x => x.MostRecentSeasons, (x, season) => new { x.Show, x.Id, MostRecentSeason = season })
                .Select(x => new { x.Show, x.Id, SeasonSummary = GetSeason(x.Id, x.MostRecentSeason)})
                .Select(x => new { x.Show, x.Id, x.SeasonSummary.SeasonNumber, Episodes = GetRecentWantedEpisodes(x.SeasonSummary.Episodes) })
                .ToArray();

            var result = recentWantedEpisodes.SelectMany(x => x.Episodes, (x, ep) => new Episode(x.Show.ShowName, x.Id, x.SeasonNumber, ep.EpisodeNumber)).ToArray();

            if (result.Length != 0)
            {
                foreach (var episode in result)
                {
                    _log.InfoFormat("SickBeard recent 'Wanted' episode found [ShowName: {0}, ShowId: {1}, Season: {2}, Episode: {3}]",
                        episode.ShowName, episode.ShowId, episode.SeasonNumber, episode.EpisodeNumber);
                }
            }
            else
            {
                _log.Info("No recent 'Wanted' episodes found");
            }

            return result;
        }

        IEnumerable<int> GetMostRecentSeasonNumber(int showId)
        {
            return _retryAction.DoWithRetries(() => _sickBeardClient.ShowSeasonList(showId), NumberOfAttempts, _delayBetweenRetries)
                .OrderByDescending(y => y)
                .Take(2);
        }

        SeasonSummary GetSeason(int showId, int seasonNumber)
        {
            return _retryAction.DoWithRetries(() => _sickBeardClient.ShowSeason(showId, seasonNumber), NumberOfAttempts, _delayBetweenRetries);
        }

        IEnumerable<EpisodeSummary> GetRecentWantedEpisodes(IEnumerable<EpisodeSummary> episodes)
        {
            return episodes
                .Where(y => y.Status == EpisodeStatus.Wanted)
                .Where(y => y.AirDate.HasValue)
                .Where(y => DateTime.Now.Subtract(y.AirDate.Value).Days < Config.WithinDays);
        }
    }
}