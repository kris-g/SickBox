using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.TorrentPostProcessor
{
    public interface IPlexLibraryRefreshTorrentPostProcessorConfig
    {
        [Required]
        int TvLibraryId { get; }

        [Required]
        string AccessToken { get; }
    }
}