using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentFTP;
using KrisG.SickBox.Core.Configuration.FileSystem;
using KrisG.SickBox.Core.Interfaces.Data.FileSystem;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using Shaman.Types;

namespace KrisG.SickBox.Core.FileSystem
{
    [ServiceImplementation("Ftp")]
    public class FtpFileSystem : IFileSystem, IConfigurable<IFtpFileSystemConfig>
    {
        private readonly Lazy<FtpClient> _lazyClient;

        public IFtpFileSystemConfig Config { get; private set; }

        public ConnectionType Type => ConnectionType.Ftp;

        public FtpFileSystem()
        {
            _lazyClient = new Lazy<FtpClient>(BuildClient);
        }

        public Stream OpenReadStream(string path)
        {
            return _lazyClient.Value.OpenRead(path);
        }

        public Stream OpenWriteStream(string path)
        {
            return _lazyClient.Value.OpenWrite(path);
        }

        public void CompleteOperation()
        {
            _lazyClient.Value.GetReply();
        }

        public bool DeleteFile(string path)
        {
            _lazyClient.Value.DeleteFile(path);
            return true;
        }

        public IEnumerable<FileEntry> ListFiles(string directoryPath)
        {
            var ftpListItems1 = _lazyClient.Value.GetListing(directoryPath).ToArray();
            var ftpListItems2 = ftpListItems1.Where(x => x.Type == FtpFileSystemObjectType.Directory).SelectMany(x => _lazyClient.Value.GetListing(x.FullName));

            return ftpListItems1
                .Concat(ftpListItems2)
                .Where(x => x.Type == FtpFileSystemObjectType.File)
                .Select(x => new FileEntry(x.Name, x.FullName));
        }

        public IEnumerable<FileEntry> ListDirectories(string directoryPath)
        {
            return _lazyClient.Value
                .GetListing(directoryPath)
                .Where(x => x.Type == FtpFileSystemObjectType.Directory)
                .Select(x => new FileEntry(x.Name, x.FullName));
        }

        public FileSize GetFileSize(string path)
        {
            return new FileSize(_lazyClient.Value.GetFileSizeAsync(path).Result);
        }

        public string PathCombine(params string[] parts)
        {
            return "/" + string.Join("/", parts);
        }

        private FtpClient BuildClient()
        {
            var client = new FtpClient(
                Config.Host,
                Config.Port,
                new NetworkCredential(Config.Username, Config.Password));

            client.Connect();

            return client;
        }
    }
}