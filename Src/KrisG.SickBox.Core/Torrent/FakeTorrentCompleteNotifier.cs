using System.Collections.Generic;
using System.Linq;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Torrent
{
    [ServiceImplementation("Fake")]
    public class FakeTorrentCompleteNotifier : ITorrentCompleteNotifier
    {
        private readonly ILog _log;

        public FakeTorrentCompleteNotifier(ILog log)
        {
            _log = log;
        }

        public IEnumerable<TorrentDownloadResult> WaitForAll(IEnumerable<TorrentSearchResult> torrents)
        {
            var result = torrents
                .Select(x => new TorrentDownloadResult(x.Episode, x.TorrentFilePath, "downloadfilepath", "downloadfilename"))
                .ToArray();

            _log.InfoFormat("Fake waited for torrent download [{0}]", JsonConvert.SerializeObject(result));

            return result;
        }
    }
}