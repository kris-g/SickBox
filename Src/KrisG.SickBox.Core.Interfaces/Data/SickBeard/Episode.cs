using KrisG.SickBox.Core.Interfaces.SickBeard;

namespace KrisG.SickBox.Core.Interfaces.Data.SickBeard
{
    public class Episode : IEpisode
    {
        public string ShowName { get; private set; }
        public int ShowId { get; private set; }
        public int SeasonNumber { get; private set; }
        public int EpisodeNumber { get; private set; }

        public Episode(string showName, int showId, int seasonNumber, int episodeNumber)
        {
            ShowName = showName;
            ShowId = showId;
            SeasonNumber = seasonNumber;
            EpisodeNumber = episodeNumber;
        }

        public override string ToString()
        {
            return string.Format("{0} S{1:00}E{2:00}", ShowName, SeasonNumber, EpisodeNumber);
        }
    }
}