using System;
using System.IO;
using System.Net;
using KrisG.Utility.Interfaces.Web;

namespace KrisG.Utility.Web
{
    public class WebStreamProvider : IWebStreamProvider
    {
        public TimeSpan Timeout { get; set; }

        public WebStreamProvider()
        {
            Timeout = TimeSpan.FromSeconds(10);
        }

        public Stream GetStream(string url)
        {
            var webRequest = WebRequest.Create(url);
            webRequest.Timeout = (int) Timeout.TotalMilliseconds;
            var webStream = webRequest.GetResponse().GetResponseStream();
            return webStream;
        }
    }
}