using System.Collections.Generic;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.Torrent
{
    [ServiceInterface("TorrentPostProcessor")]
    public interface ITorrentPostProcessor
    {
        void PostProcess(IEnumerable<TorrentDownloadResult> torrents);
    }
}