using System.Collections.Generic;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.Torrent
{
    [ServiceInterface("TorrentCompleteNotifier")]
    public interface ITorrentCompleteNotifier
    {
        IEnumerable<TorrentDownloadResult> WaitForAll(IEnumerable<TorrentSearchResult> torrents);
    }
}