using System;
using System.IO;
using System.Net;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client.Internal
{
    internal class WebStreamProvider : IWebStreamProvider
    {
        private Lazy<WebClient> _webClient = new Lazy<WebClient>(() => new WebClient());

        public Stream GetStream(string url)
        {
            return _webClient.Value.OpenRead(url);
        }

        public void DownloadFile(string url, string localFilePath)
        {
            _webClient.Value.DownloadFile(new Uri(url), localFilePath);
        }

        public void Dispose()
        {
            // should do proper dispose pattern
            if (_webClient.IsValueCreated)
            {
                _webClient.Value.Dispose();
                _webClient = null;
            }
        }
    }
}