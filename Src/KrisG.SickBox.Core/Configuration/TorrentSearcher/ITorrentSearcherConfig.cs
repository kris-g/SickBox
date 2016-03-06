namespace KrisG.SickBox.Core.Configuration.TorrentSearcher
{
    public interface ITorrentSearcherConfig
    {
        string[] SearchQueryAdditions { get; }
        string[] CategoriesToExclude { get; }        
    }
}