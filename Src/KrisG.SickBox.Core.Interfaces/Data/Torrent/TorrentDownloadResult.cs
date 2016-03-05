using KrisG.SickBox.Core.Interfaces.SickBeard;

namespace KrisG.SickBox.Core.Interfaces.Data.Torrent
{
    public class TorrentDownloadResult : TorrentSearchResult
    {
        public string DownloadFilePath { get; private set; }
        public string DownloadFileName { get; private set; }

        public TorrentDownloadResult(IEpisode episode, string torrentFilePath, string downloadFilePath, string downloadFileName)
            : base(episode, torrentFilePath)
        {
            DownloadFileName = downloadFileName;
            DownloadFilePath = downloadFilePath;
        }
    }
}