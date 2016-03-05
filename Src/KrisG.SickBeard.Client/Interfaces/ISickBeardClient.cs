using System.Collections.Generic;
using KrisG.SickBeard.Client.Data;

namespace KrisG.SickBeard.Client.Interfaces
{
    public interface ISickBeardClient
    {
        void Initialise(string url, string apiKey);
        IEnumerable<Show> Shows();
        SeasonSummary ShowSeason(int id, int seasonNumber);
        IEnumerable<HistoryEntry> History(int? limit);
        ShowDetails Show(int id);
        bool ShowRefresh(int id);
        void ShowFixFileNames(int id);
        IEnumerable<int> ShowSeasonList(int id);
    }
}