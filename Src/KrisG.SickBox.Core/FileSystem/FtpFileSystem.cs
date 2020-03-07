using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using ArxOne.Ftp;
using KrisG.SickBox.Core.Configuration.FileSystem;
using KrisG.SickBox.Core.Interfaces.Data.FileSystem;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;

namespace KrisG.SickBox.Core.FileSystem
{
    [ServiceImplementation("Ftp")]
    public class FtpFileSystem : IFileSystem, IConfigurable<IFtpFileSystemConfig>
    {
        private readonly Lazy<FtpClient> _lazyClient;

        public IFtpFileSystemConfig Config { get; private set; }

        public ConnectionType Type
        {
            get { return ConnectionType.Ftp; }
        }

        public FtpFileSystem()
        {
            _lazyClient = new Lazy<FtpClient>(BuildClient);
        }

        public Stream OpenReadStream(string path)
        {
            return _lazyClient.Value.Retr(new FtpPath(path));
        }

        public Stream OpenWriteStream(string path)
        {
            return _lazyClient.Value.Stor(new FtpPath(path));
        }

        public void CompleteOperation()
        {
        }

        public bool DeleteFile(string path)
        {
            return _lazyClient.Value.Dele(new FtpPath(path));
        }

        public IEnumerable<FileEntry> ListFiles(string directoryPath)
        {
            return _lazyClient.Value
                .ListEntries(new FtpPath(directoryPath))
                .Where(x => x.Type == FtpEntryType.File)
                .Select(x => new FileEntry(x.Name, x.Path.ToString()));
        }

        public IEnumerable<FileEntry> ListDirectories(string directoryPath)
        {
            return _lazyClient.Value
                .ListEntries(new FtpPath(directoryPath))
                .Where(x => x.Type == FtpEntryType.Directory)
                .Select(x => new FileEntry(x.Name, x.Path.ToString()));
        }

        public FileSize GetFileSize(string path)
        {
            var ftpEntry = _lazyClient.Value.GetEntry(new FtpPath(path));

            if (ftpEntry != null)
            {
                return new FileSize(ftpEntry.Size.Value);
            }

            return new FileSize();
        }

        public string PathCombine(params string[] parts)
        {
            return string.Join("/", parts);
        }

        private FtpClient BuildClient()
        {
            return new FtpClient(FtpProtocol.Ftp, Config.Host, Config.Port, new NetworkCredential(Config.Username, Config.Password));
        }
    }
}