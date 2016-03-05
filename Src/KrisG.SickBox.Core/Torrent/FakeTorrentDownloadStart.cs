using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Torrent
{
    [ServiceImplementation("Fake")]
    public class FakeTorrentDownloadStart : ITorrentDownloadStart
    {
        private readonly ILog _log;

        public FakeTorrentDownloadStart(ILog log)
        {
            _log = log;
        }

        public void StartDownload(TorrentSearchResult[] torrents)
        {
            _log.InfoFormat("Fake starting torrent downloads [{0}]", JsonConvert.SerializeObject(torrents));
        }
    }
}