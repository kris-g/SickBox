using System;
using KrisG.SickBeard.Client.Enums;

namespace KrisG.SickBeard.Client.Data
{
    public class HistoryEntry
    {
        public int ShowId { get; set; }
        public string ShowName { get; private set; }
        public int SeasonNumber { get; private set; }
        public int EpisodeNumber { get; private set; }
        public DateTime Date { get; private set; }
        public string Quality { get; private set; }
        public EpisodeStatus Status { get; private set; }

        public HistoryEntry(int showId, string showName, int seasonNumber, int episodeNumber, DateTime date, string quality, EpisodeStatus status)
        {
            ShowId = showId;
            ShowName = showName;
            SeasonNumber = seasonNumber;
            EpisodeNumber = episodeNumber;
            Date = date;
            Quality = quality;
            Status = status;
        }
    }
}