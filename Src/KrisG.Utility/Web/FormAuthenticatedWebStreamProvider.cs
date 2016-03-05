using System.Collections.Specialized;
using System.IO;
using System.Text;
using KrisG.Utility.Interfaces.Web;
using KrisG.Utility.Internal.Web;

namespace KrisG.Utility.Web
{
    public class FormAuthenticatedWebStreamProvider : IFormAuthenticatedWebStreamProvider
    {
        private CookieAwareWebClient _webClient;

        private CookieAwareWebClient WebClient
        {
            get { return _webClient ?? (_webClient = BuildAuthenticatedWebClient()); }
        }

        public string Username { get; set; }

        public string Password { get; set; }

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
            var url = @"https://iptorrents.eu/t?q=big+bang+theory+720p&qf=#torrents";

            var client = new CookieAwareWebClient();

            var values = new NameValueCollection
            {
                {"username", Username},
                {"password", Password},
            };

            var response = client.UploadValues(url, "POST", values);

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