using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.Torrent
{
    [ServiceInterface("TorrentSearcher")]
    public interface ITorrentSearcher
    {
        TorrentSearchResult Search(IEpisode episode);
    }
}