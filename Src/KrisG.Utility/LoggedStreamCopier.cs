using System.Diagnostics;
using System.IO;
using KrisG.Utility.Interfaces;
using log4net;

namespace KrisG.Utility
{
    public class LoggedStreamCopier : IStreamCopier
    {
        private const int _defaultCopyBufferSize = 81920;

        private readonly ILog _log;
        private int _lookingForPosition;

        public LoggedStreamCopier(ILog log)
        {
            _log = log;
        }

        public void Copy(Stream from, Stream to, string toPath, long totalSize)
        {
            var totalSizeEx = new FileSize(totalSize);

            var sw = new Stopwatch();
            sw.Start();

            _lookingForPosition = 1;

            var buffer = new byte[_defaultCopyBufferSize];
            int read, totalRead = 0;
            while ((read = from.Read(buffer, 0, buffer.Length)) > 0)
            {
                to.Write(buffer, 0, read);

                totalRead += read;
                LogProgress(toPath, totalRead, totalSize, sw);
            }

            CompleteLog(toPath, totalSizeEx, sw);
        }

        private void LogProgress(string toPath, int currentPosition, long totalSize, Stopwatch sw)
        {
            var percentage = ((double) currentPosition / totalSize) * 100;

            if (percentage > _lookingForPosition * 10)
            {
                var transferSpeed = new FileSize((long)(currentPosition / sw.Elapsed.TotalSeconds));

                _log.InfoFormat("Data copying {0:00.00}% [RemotePath: {1}, CopyTime: {2}, TransferSpeed: {3}/s]", 
                    percentage, toPath, sw.Elapsed, transferSpeed);
                    
                _lookingForPosition = ((int) percentage / 10) + 1;
            }
        }

        private void CompleteLog(string toPath, FileSize totalSize, Stopwatch sw)
        {
            sw.Stop();
            var transferSpeed = new FileSize((long)(totalSize.Bytes / sw.Elapsed.TotalSeconds));
            
            _log.InfoFormat("Data copy finished [RemotePath: {0}, Size: {1}, CopyTime: {2}, TransferSpeed: {3}/s]",
                toPath, totalSize, sw.Elapsed, transferSpeed);
        }
    }
}