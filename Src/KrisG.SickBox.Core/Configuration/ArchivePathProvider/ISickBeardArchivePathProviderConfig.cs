using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.ArchivePathProvider
{
    public interface ISickBeardArchivePathProviderConfig
    {
        [Required]
        string SickBeardRootPath { get; }
    }
}