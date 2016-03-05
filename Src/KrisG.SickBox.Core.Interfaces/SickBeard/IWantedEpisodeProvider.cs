using System.Collections.Generic;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.SickBeard
{
    [ServiceInterface("WantedEpisodeProviders")]
    public interface IWantedEpisodeProvider
    {
        IEnumerable<IEpisode> FetchEpisodes();
    }
}