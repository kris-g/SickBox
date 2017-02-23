namespace KrisG.SickBox.Core.Configuration.TorrentSearcher
{
    public interface ITorrentSearcherConfig : ITorrentSearcherConfigBase
    {
        ITorrentSearcherEpisodeOverrideConfig[] EpisodeOverrides { get; }
    }
}