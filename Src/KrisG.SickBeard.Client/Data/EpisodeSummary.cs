using System;
using KrisG.SickBeard.Client.Enums;

namespace KrisG.SickBeard.Client.Data
{
    public class EpisodeSummary
    {
        public int EpisodeNumber { get; private set; }
        public string Name { get; private set; }
        public DateTime? AirDate { get; private set; }
        public string Quality { get; private set; }
        public EpisodeStatus Status { get; private set; }

        public EpisodeSummary(int episodeNumber, string name, DateTime? airDate, string quality, EpisodeStatus status)
        {
            EpisodeNumber = episodeNumber;
            Name = name;
            AirDate = airDate;
            Quality = quality;
            Status = status;
        }
    }
}