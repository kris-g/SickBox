using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KrisG.Utility.Interfaces;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.PathProvider;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using log4net;
using Shaman.Types;

namespace KrisG.SickBox.Core.Torrent
{
    [ServiceImplementation("FileMoveAndDelete")]
    public class SingleFileMoveTorrentArchiver : ITorrentDownloadArchiver
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IArchivePathProvider _archivePathProvider;
        private readonly IStreamCopier _streamCopier;
        private readonly ILog _log;

        public SingleFileMoveTorrentArchiver(
            IFileSystemProvider fileSystemProvider,
            IArchivePathProvider archivePathProvider,
            IStreamCopier streamCopier,
            ILog log)
        {
            _fileSystemProvider = fileSystemProvider;
            _archivePathProvider = archivePathProvider;
            _streamCopier = streamCopier;
            _log = log;
        }

        public void Finalise(IEnumerable<TorrentDownloadResult> torrents)
        {
            var torrentsArray = torrents.ToArray();

            var torrentFileSystem = _fileSystemProvider.Get(FileSystemContext.TorrentDownloader);
            var archiveFileSystem = _fileSystemProvider.Get(FileSystemContext.ArchiveArea);

            var showIdToPath = new Dictionary<int, string>();

            foreach (var entry in torrentsArray)
            {
                var path = _archivePathProvider.GetPath(entry.Episode);

                if (path == null)
                {
                    continue;
                }

                showIdToPath[entry.Episode.ShowId] = path;
            }

            _log.InfoFormat("Starting {0} file copy operations to archive area", torrentsArray.Length);

            foreach (var entry in torrentsArray)
            {
                if (!showIdToPath.ContainsKey(entry.Episode.ShowId))
                {
                    continue;
                }

                var fileSize = torrentFileSystem.GetFileSize(entry.DownloadFilePath);

                var archiveFolder = showIdToPath[entry.Episode.ShowId];
                var archiveFileName = entry.DownloadFileName;
                var archiveFilePath = archiveFileSystem.PathCombine(archiveFolder, archiveFileName);

                _log.InfoFormat("Starting file copy [From: {0} {1}, To: {2} {3}]", torrentFileSystem.Type, entry.DownloadFilePath, archiveFileSystem.Type, archiveFilePath);

                var sw = new Stopwatch();
                sw.Start();
                using (var ftpStream = OpenReadStream(torrentFileSystem, entry))
                {
                    using (var fileStream = OpenWriteStream(archiveFileSystem, archiveFilePath))
                    {
                        _streamCopier.Copy(ftpStream, fileStream, entry.DownloadFilePath, fileSize.Bytes);
                    }
                }
                sw.Stop();
                torrentFileSystem.CompleteOperation();
                archiveFileSystem.CompleteOperation();

                var transferSpeed = new FileSize((long)(fileSize.Bytes / sw.Elapsed.TotalSeconds));
                _log.InfoFormat("File copy finished [RemotePath: {0}, Size: {1}, CopyTime: {2}, TransferSpeed: {3}/s]",
                   entry.DownloadFilePath, fileSize, sw.Elapsed, transferSpeed);

                _log.InfoFormat("Deleting torrent download file [{0}]", entry.DownloadFilePath);
                torrentFileSystem.DeleteFile(entry.DownloadFilePath);
            }

            _log.InfoFormat("Finished archive file copy operations");

        }

        private Stream OpenReadStream(IFileSystem fileSystem, TorrentDownloadResult entry)
        {
            try
            {
                return fileSystem.OpenReadStream(entry.DownloadFilePath);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to open read stream", ex);
                throw;
            }
        }

        private Stream OpenWriteStream(IFileSystem fileSystem, string filePath)
        {
            try
            {
                return fileSystem.OpenWriteStream(filePath);
            }
            catch (Exception ex)
            {
                _log.Error("Failed to open write stream", ex);
                throw;
            }
        }
    }
}