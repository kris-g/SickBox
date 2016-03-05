using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.WantedEpisodeProvider
{
    public interface IRecentWantedEpisodeProviderConfig
    {
        [DefaultValue(10)]
        int WithinDays { get; } 
    }
}