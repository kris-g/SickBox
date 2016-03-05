using KrisG.SickBox.Core.Interfaces.SickBeard;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Interfaces.Data.Torrent
{
    public class TorrentSearchResult
    {
        public IEpisode Episode { get; private set; }
        public string TorrentFilePath { get; private set; }

        [JsonConstructor]
        public TorrentSearchResult(IEpisode episode, string torrentFilePath)
        {
            Episode = episode;
            TorrentFilePath = torrentFilePath;
        }

        public override string ToString()
        {
            return string.Format("{0} S{1:00}E{2:00}", Episode.ShowName, Episode.SeasonNumber, Episode.EpisodeNumber);
        }
    }
}