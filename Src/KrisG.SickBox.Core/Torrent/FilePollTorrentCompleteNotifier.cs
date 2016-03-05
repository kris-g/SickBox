using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using KrisG.SickBox.Core.Configuration.TorrentCompleteNotifier;
using KrisG.SickBox.Core.Interfaces.Data.Torrent;
using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.FileSystem;
using KrisG.SickBox.Core.Interfaces.SickBeard;
using KrisG.SickBox.Core.Interfaces.Torrent;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using log4net;
using Newtonsoft.Json;

namespace KrisG.SickBox.Core.Torrent
{
    [ServiceImplementation("PollForCompleteFiles")]
    public class FilePollTorrentCompleteNotifier : ITorrentCompleteNotifier, IConfigurable<IFilePollTorrentCompleteNotifierConfig>
    {
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IEpisodeMatcher _episodeMatcher;
        private readonly ILog _log;

        public IFilePollTorrentCompleteNotifierConfig Config { get; private set; }

        public FilePollTorrentCompleteNotifier(
            IFileSystemProvider fileSystemProvider,
            IEpisodeMatcher episodeMatcher,
            ILog log)
        {
            _fileSystemProvider = fileSystemProvider;
            _episodeMatcher = episodeMatcher;
            _log = log;
        }

        public IEnumerable<TorrentDownloadResult> WaitForAll(IEnumerable<TorrentSearchResult> torrents)
        {
            if (Config.CompleteFilePossibleDestinationPaths == null || Config.CompleteFilePossibleDestinationPaths.Length == 0)
            {
                throw new InvalidOperationException("No valid configuration set for CompleteFilePossibleDestinationPaths");
            }

            var torrentsArray = torrents.ToArray();
            var result = new List<TorrentDownloadResult>();

            var fileSystem = _fileSystemProvider.Get(FileSystemContext.TorrentDownloader);

            _log.InfoFormat("Starting polling of destination folders looking for complete torrent downloads");

            var sw = new Stopwatch();
            sw.Start();

            var pollAttempt = 0;
            int pollInterval = Config.FileArrivePollIntervalSeconds * 1000;
            int maxPollAttempts = (Config.FileArrivePollTotalTimeMinutes*60*1000)/pollInterval;
            while (pollAttempt < maxPollAttempts && result.Count != torrentsArray.Length)
            {
                pollAttempt++;
                _log.InfoFormat("Poll attempt {0} of {1} [Duration: {2}]", pollAttempt, maxPollAttempts, sw.Elapsed);

                var downloadItems = Config.CompleteFilePossibleDestinationPaths.SelectMany(fileSystem.ListFiles);
                var downloadItemsToCheck = downloadItems
                    .Where(x => !result.Any(tdr => tdr.DownloadFilePath == x.Path))
                    .ToArray();

                foreach (var fileEntry in downloadItemsToCheck)
                {
                    var remainingTorrents = torrentsArray.Where(x => !result.Select(y => y.Episode).Contains(x.Episode));
                    foreach (var candidate in remainingTorrents)
                    {
                        var entry = candidate.Episode;

                        if (_episodeMatcher.IsMatch(fileEntry.Name, entry))
                        {
                            _log.InfoFormat("Matched downloaded file {0} to episode item [{1}]", fileEntry.Name, JsonConvert.SerializeObject(entry, Formatting.None));

                            result.Add(new TorrentDownloadResult(entry, candidate.TorrentFilePath, fileEntry.Path, fileEntry.Name));
                            break;
                        }
                    }
                }

                if (result.Count != torrentsArray.Length)
                {
                    Thread.Sleep(pollInterval);
                }
            }

            if (pollAttempt == maxPollAttempts)
            {
                _log.InfoFormat("Max poll attempts reached");
            }

            if (result.Count != 0)
            {
                _log.InfoFormat("{0} items ready for archive/download, polling to check when files stop growing", result.Count);

                bool fileSizeChanged = true;
                var fileSizes = new Dictionary<string, long>();

                pollAttempt = 0;
                pollInterval = Config.FileGrowingPollIntervalSeconds * 1000;
                maxPollAttempts = (Config.FileGrowingPollTotalTimeMinutes*60*1000)/pollInterval;

                while (pollAttempt < maxPollAttempts && fileSizeChanged)
                {
                    pollAttempt++;

                    var allSizeChanged = new List<bool>();
                    foreach (var item in result)
                    {
                        long oldSize = -1;
                        var newSize = fileSystem.GetFileSize(item.DownloadFilePath);

                        if (fileSizes.ContainsKey(item.DownloadFilePath))
                        {
                            oldSize = fileSizes[item.DownloadFilePath];
                        }
                        fileSizes[item.DownloadFilePath] = newSize.Bytes;

                        allSizeChanged.Add(oldSize != newSize.Bytes);
                    }

                    fileSizeChanged = allSizeChanged.Any(x => x);

                    _log.InfoFormat("Any file sizes changed check [{0}]", fileSizeChanged ? "True" : "False");

                    Thread.Sleep(pollInterval);
                }

                if (pollAttempt == maxPollAttempts)
                {
                    _log.InfoFormat("Max poll attempts reached");
                }
            }

            return result;
        }
    }
}