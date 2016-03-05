using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.WantedEpisodeProvider
{
    public interface ISnatchedButStillMissingEpisodeProviderConfig
    {
        [DefaultValue(10)]
        int HistoryItems { get; } 
    }
}