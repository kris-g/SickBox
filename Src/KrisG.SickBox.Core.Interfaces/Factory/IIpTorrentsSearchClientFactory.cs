using KrisG.IpTorrents.Client.Interfaces;

namespace KrisG.SickBox.Core.Interfaces.Factory
{
    public interface IIpTorrentsSearchClientFactory
    {
        ITorrentSearchClient GetClient();
    }
}