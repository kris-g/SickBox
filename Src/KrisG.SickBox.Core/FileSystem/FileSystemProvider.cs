using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.Utility.Interfaces.Configuration;
using KrisG.Utility.Interfaces.Service;

namespace KrisG.SickBox.Core.FileSystem
{
    public class FileSystemProvider : IFileSystemProvider
    {
        private readonly IConfigurationReader _configReader;
        private readonly IServiceResolver _serviceResolver;

        public FileSystemProvider(IConfigurationReader configReader, IServiceResolver serviceResolver)
        {
            _configReader = configReader;
            _serviceResolver = serviceResolver;
        }

        public IFileSystem Get(FileSystemContext context)
        {
            var doc = _configReader.Read();
            var fileSystemElement = doc.Root.Element("Connections").Element(context.ToString());

            var fileSystem = _serviceResolver.Get<IFileSystem>(fileSystemElement.Attribute("Type").Value, fileSystemElement);

            return fileSystem;
        }
    }
}