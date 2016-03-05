using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.Torrent
{
    [ServiceInterface("TorrentDownloadStart")]
    public interface ITorrentDownloadStart
    {
        void StartDownload(TorrentSearchResult[] torrents);
    }
}