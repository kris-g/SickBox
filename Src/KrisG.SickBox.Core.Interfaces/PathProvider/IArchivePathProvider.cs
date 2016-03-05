using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;

namespace KrisG.SickBox.Core.Interfaces.PathProvider
{
    [ServiceInterface("ArchivePath")]
    public interface IArchivePathProvider
    {
        string GetPath(IEpisode episode);
    }
}