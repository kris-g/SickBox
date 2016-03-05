using System.Collections.Generic;
using KrisG.SickBox.Core.Configuration.WantedEpisodeProvider;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;

namespace KrisG.SickBox.Core.WantedEpisodeProvider
{
    [ServiceImplementation("Fake")]
    public class FakeEpisodeProvider : IWantedEpisodeProvider, IConfigurable<IFakeEpisodeProvider>
    {
        public IFakeEpisodeProvider Config { get; private set; }

        public IEnumerable<IEpisode> FetchEpisodes()
        {
            return Config.Episodes;
        }
    }
}