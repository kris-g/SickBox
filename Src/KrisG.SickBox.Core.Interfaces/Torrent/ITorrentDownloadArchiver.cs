using System.Collections.Generic;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.Torrent
{
    [ServiceInterface("TorrentDownloadArchiver")]
    public interface ITorrentDownloadArchiver
    {
        void Finalise(IEnumerable<TorrentDownloadResult> torrents);
    }
}