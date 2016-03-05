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
    public class FakeTorrentArchiver : ITorrentDownloadArchiver
    {
        private readonly ILog _log;

        public FakeTorrentArchiver(ILog log)
        {
            _log = log;
        }

        public void Finalise(IEnumerable<TorrentDownloadResult> torrents)
        {
            _log.InfoFormat("Fake torrent archive [{0}]", JsonConvert.SerializeObject(torrents.ToArray()));            
        }
    }
}