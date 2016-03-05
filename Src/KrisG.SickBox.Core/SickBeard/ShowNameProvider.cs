using System.Linq;
using KrisG.SickBox.Core.Configuration.ShowNameProvider;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;

namespace KrisG.SickBox.Core.SickBeard
{
    [ServiceImplementation("ShowName")]
    public class ShowNameProvider : IShowNameProvider, IConfigurable<IShowNameProviderConfig>
    {
        public IShowNameProviderConfig Config { get; private set; }

        public string Get(IEpisode episode)
        {
            var definedOverrides = new IShowNameOverride[0];

            if (Config != null && Config.Overrides != null)
            {
                definedOverrides = Config.Overrides;
            }

            var overrides = definedOverrides
                .ToLookup(x => x.Id)
                .ToDictionary(x => x.Key, x => x.First().ShowName);

            if (overrides.ContainsKey(episode.ShowId))
            {
                return overrides[episode.ShowId];
            }

            return episode.ShowName;
        }
    }
}