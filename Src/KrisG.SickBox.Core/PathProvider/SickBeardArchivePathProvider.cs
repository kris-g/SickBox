using System.Linq;
using KrisG.SickBox.Core.Configuration.ArchivePathProvider;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Factory;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.PathProvider;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.PathProvider
{
    [ServiceImplementation("SickBeardArchive")]
    public class SickBeardArchivePathProvider : IArchivePathProvider, IConfigurable<ISickBeardArchivePathProviderConfig>
    {
        private readonly ISickBeardClientFactory _clientFactory;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly ILog _log;

        public ISickBeardArchivePathProviderConfig Config { get; private set; }

        public SickBeardArchivePathProvider(
            ISickBeardClientFactory clientFactory,
            IFileSystemProvider fileSystemProvider,
            ILog log)
        {
            _clientFactory = clientFactory;
            _fileSystemProvider = fileSystemProvider;
            _log = log;
        }

        public string GetPath(IEpisode episode)
        {
            var fileSystem = _fileSystemProvider.Get(FileSystemContext.ArchiveArea);
            var pathComponents = Config.SickBeardRootPath.Split('\\', '/');
            
            _log.InfoFormat("Querying SickBeard to find show folders");
            var showDetails = _clientFactory.GetClient().Show(episode.ShowId);

            if (showDetails == null)
            {
                _log.WarnFormat("Failed to get show details from SickBeard [{0}]", JsonConvert.SerializeObject(episode, Formatting.None));
                return null;
            }

            var folder = showDetails.Location.Split('\\', '/').Last();
            return fileSystem.PathCombine(pathComponents.Concat(new[] {folder}).ToArray());
        }
    }
}