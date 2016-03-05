using System.Collections.Generic;

namespace KrisG.SickBeard.Client.Data
{
    public class SeasonSummary
    {
        public int Id { get; private set; }
        public int SeasonNumber { get; private set; } 
        public IEnumerable<EpisodeSummary> Episodes { get; private set; }

        public SeasonSummary(int id, int seasonNumber, IEnumerable<EpisodeSummary> episodes)
        {
            Id = id;
            SeasonNumber = seasonNumber;
            Episodes = episodes;
        }
    }
}