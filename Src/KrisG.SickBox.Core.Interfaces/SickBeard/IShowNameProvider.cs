using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.SickBeard
{
    [ServiceInterface("ShowName")]
    public interface IShowNameProvider
    {
        string Get(IEpisode episode);
    }
}