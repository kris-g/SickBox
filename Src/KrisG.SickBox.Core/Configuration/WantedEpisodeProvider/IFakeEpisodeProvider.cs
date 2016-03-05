using KrisG.SickBox.Core.Interfaces.SickBeard;

namespace KrisG.SickBox.Core.Configuration.WantedEpisodeProvider
{
    public interface IFakeEpisodeProvider
    {
        IEpisode[] Episodes { get; } 
    }
}