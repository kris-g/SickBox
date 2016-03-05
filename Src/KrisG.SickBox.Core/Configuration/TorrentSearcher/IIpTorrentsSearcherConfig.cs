namespace KrisG.SickBox.Core.Configuration.TorrentSearcher
{
    public interface IIpTorrentsSearcherConfig
    {
        string[] SearchQueryAdditions { get; }
        string[] CategoriesToExclude { get; }        
    }
}