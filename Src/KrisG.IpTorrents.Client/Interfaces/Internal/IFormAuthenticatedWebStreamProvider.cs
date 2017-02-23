using System;
using System.IO;

namespace KrisG.IpTorrents.Client.Interfaces.Internal
{
    internal interface IFormAuthenticatedWebStreamProvider : IDisposable
    {
        string Username { get; set; }

        string Password { get; set; }
        
        string FormAuthUrl { get; set; }

        IProxyConfig ProxyConfig { get; set; }

        Stream GetStream(string url);

        void DownloadFile(string url, string localFilePath);
    }
}