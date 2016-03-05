using KrisG.SickBox.Core.Configuration.ArchivePathProvider;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.PathProvider;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;

namespace KrisG.SickBox.Core.PathProvider
{
    [ServiceImplementation("RootFolderArchive")]
    public class RootFolderArchivePathProvider : IArchivePathProvider, IConfigurable<IArchivePathProviderConfig>
    {
        private readonly IFileSystemProvider _fileSystemProvider;

        public IArchivePathProviderConfig Config { get; private set; }
        
        public RootFolderArchivePathProvider(IFileSystemProvider fileSystemProvider)
        {
            _fileSystemProvider = fileSystemProvider;
        }

        public string GetPath(IEpisode episode)
        {
            var fileSystem = _fileSystemProvider.Get(FileSystemContext.ArchiveArea);
            var pathComponents = Config.RootPath.Split('\\', '/');

            return fileSystem.PathCombine(pathComponents);
        }
    }
}