using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.TorrentDownloadStart
{
    public interface IWatchFolderTorrentDownloadStartConfig
    {
        [Required]
        string WatchFolderPath { get; }         
    }
}