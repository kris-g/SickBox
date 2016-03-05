using System;
using KrisG.SickBeard.Client.Enums;

namespace KrisG.SickBeard.Client.Data
{
    public class Show
    {
        public int Id { get; set; }
        public bool AirByDate { get; set; }
        public string Language { get; set; }
        public string Network { get; set; }
        public DateTime? NextEpAirDate { get; set; }
        public bool Paused { get; set; }
        public string Quality { get; set; }
        public string ShowName { get; set; }       
        public ShowStatus Status { get; set; }
        public int TvRageId { get; set; }
        public string TvRageName { get; set; }

        public Show(int id, bool airByDate, string language, string network, DateTime? nextEpAirDate, bool paused, string quality, string showName, ShowStatus status, int tvRageId, string tvRageName)
        {
            Id = id;
            AirByDate = airByDate;
            Language = language;
            Network = network;
            NextEpAirDate = nextEpAirDate;
            Paused = paused;
            Quality = quality;
            ShowName = showName;
            Status = status;
            TvRageId = tvRageId;
            TvRageName = tvRageName;
        }
    }
}