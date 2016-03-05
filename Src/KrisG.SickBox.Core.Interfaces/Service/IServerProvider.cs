using KrisG.SickBox.Core.Interfaces.Enums;

namespace KrisG.SickBox.Core.Interfaces.Service
{
    public interface IServerProvider
    {
        TConfig Get<TConfig>(ServerType type);
    }
}