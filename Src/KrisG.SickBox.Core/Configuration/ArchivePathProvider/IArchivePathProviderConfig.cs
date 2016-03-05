using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Configuration.ArchivePathProvider
{
    public interface IArchivePathProviderConfig
    {
        [Required]
        string RootPath { get; }
    }
}