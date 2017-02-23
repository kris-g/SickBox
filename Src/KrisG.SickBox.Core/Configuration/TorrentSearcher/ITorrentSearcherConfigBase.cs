namespace KrisG.SickBox.Core.Configuration.TorrentSearcher
{
    public interface ITorrentSearcherConfigBase
    {
        string[] SearchQueryAdditions { get; }
        string[] CategoriesToExclude { get; }
    }
}