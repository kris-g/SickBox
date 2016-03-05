using KrisG.SickBox.Core.Interfaces.Enums;

namespace KrisG.SickBox.Core.Interfaces.FileSystem
{
    public interface IFileSystemProvider
    {
        IFileSystem Get(FileSystemContext context);
    }
}