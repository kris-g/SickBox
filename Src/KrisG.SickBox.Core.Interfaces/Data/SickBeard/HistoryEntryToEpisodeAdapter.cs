using KrisG.SickBeard.Client.Data;
using KrisG.SickBox.Core.Interfaces.SickBeard;

namespace KrisG.SickBox.Core.Interfaces.Data.SickBeard
{
    public class HistoryEntryToEpisodeAdapter : IEpisode
    {
        private readonly HistoryEntry _innerEntry;

        public string ShowName
        {
            get { return _innerEntry.ShowName; }
        }

        public int ShowId
        {
            get { return _innerEntry.ShowId; }
        }

        public int SeasonNumber
        {
            get { return _innerEntry.SeasonNumber; }
        }

        public int EpisodeNumber
        {
            get { return _innerEntry.EpisodeNumber; }
        }

        public HistoryEntryToEpisodeAdapter(HistoryEntry innerEntry)
        {
            _innerEntry = innerEntry;
        }

        public override string ToString()
        {
            return string.Format("{0} S{1:00}E{2:00}", ShowName, SeasonNumber, EpisodeNumber);
        }
    }
}