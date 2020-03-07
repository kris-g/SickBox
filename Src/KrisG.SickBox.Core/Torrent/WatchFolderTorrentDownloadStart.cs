using System;
using System.IO;
using KrisG.SickBox.Core.Configuration.TorrentDownloadStart;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;

namespace KrisG.SickBox.Core.Torrent
{
    [ServiceImplementation("WatchFolder")]
    public class WatchFolderTorrentDownloadStart : ITorrentDownloadStart, IConfigurable<IWatchFolderTorrentDownloadStartConfig>
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly ILog _log;

        public IWatchFolderTorrentDownloadStartConfig Config { get; private set; }

        public WatchFolderTorrentDownloadStart(IFileSystemProvider fileSystemProvider, ILog log)
        {
            _fileSystemProvider = fileSystemProvider;
            _log = log;
        }

        public void StartDownload(TorrentSearchResult[] torrents)
        {
            if (string.IsNullOrWhiteSpace(Config.WatchFolderPath))
            {
                throw new InvalidOperationException("No valid watch folder set in configuration");
            }

            var fileSystem = _fileSystemProvider.Get(FileSystemContext.TorrentDownloader);

            _log.InfoFormat("Connecting to {0} server for uploading {1} torrent(s) to watch folder '{2}'", fileSystem.Type, torrents.Length, Config.WatchFolderPath);

            foreach (var item in torrents)
            {
                var fileName = Path.GetFileName(item.TorrentFilePath);
                var remotePath = fileSystem.PathCombine(Config.WatchFolderPath, fileName);

                using (var fileStream = File.OpenRead(item.TorrentFilePath))
                {
                    using (var ftpStream = fileSystem.OpenWriteStream(remotePath))
                    {
                        fileStream.CopyTo(ftpStream);
                    }
                    fileSystem.CompleteOperation();
                }
            }
        }
    }
}