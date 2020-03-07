using System;
using System.IO;

namespace KrisG.IpTorrents.Client.Interfaces.Internal
{
    internal interface IWebStreamProvider : IDisposable
    {
        Stream GetStream(string url);

        void DownloadFile(string url, string localFilePath);
    }
}