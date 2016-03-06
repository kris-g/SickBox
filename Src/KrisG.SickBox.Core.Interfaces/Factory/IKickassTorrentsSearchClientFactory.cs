using KrisG.KickassTorrents.Client.Interfaces;

namespace KrisG.SickBox.Core.Interfaces.Factory
{
    public interface IKickassTorrentsSearchClientFactory
    {
        ITorrentSearchClient GetClient();         
    }
}