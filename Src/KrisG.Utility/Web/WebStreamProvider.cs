using System.IO;
using System.Net;
using KrisG.Utility.Interfaces.Web;

namespace KrisG.Utility.Web
{
    public class WebStreamProvider : IWebStreamProvider
    {
        public Stream GetStream(string url)
        {
            var webRequest = WebRequest.Create(url);
            var webStream = webRequest.GetResponse().GetResponseStream();
            return webStream;
        }
    }
}