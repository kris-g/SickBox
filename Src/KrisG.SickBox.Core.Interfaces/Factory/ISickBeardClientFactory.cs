using KrisG.SickBeard.Client.Interfaces;

namespace KrisG.SickBox.Core.Interfaces.Factory
{
    public interface ISickBeardClientFactory
    {
        ISickBeardClient GetClient();
    }
}