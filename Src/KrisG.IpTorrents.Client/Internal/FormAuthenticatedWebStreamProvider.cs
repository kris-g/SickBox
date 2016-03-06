using System.Collections.Specialized;
using System.IO;
using System.Text;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client.Internal
{
    internal class FormAuthenticatedWebStreamProvider : IFormAuthenticatedWebStreamProvider
    {
        private CookieAwareWebClient _webClient;

        private CookieAwareWebClient WebClient
        {
            get { return _webClient ?? (_webClient = BuildAuthenticatedWebClient()); }
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string FormAuthUrl { get; set; }

        public Stream GetStream(string url)
        {
            var webStream = WebClient.OpenRead(url);
            return webStream;
        }

        public void DownloadFile(string url, string localFilePath)
        {
            WebClient.DownloadFile(url, localFilePath);            
        }

        private CookieAwareWebClient BuildAuthenticatedWebClient()
        {
            var client = new CookieAwareWebClient();

            var values = new NameValueCollection
            {
                {"username", Username},
                {"password", Password},
            };

            var response = client.UploadValues(FormAuthUrl, "POST", values);

            // should allow login validation
            var responseStr = Encoding.Default.GetString(response);

            return client;
        }

        public void Dispose()
        {
            // should do proper dispose pattern
            if (_webClient != null)
            {
                _webClient.Dispose();
                _webClient = null;
            }
        }
    }
}