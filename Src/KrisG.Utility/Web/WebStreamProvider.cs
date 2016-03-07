using System.IO;
using System.Net;
using KrisG.Utility.Interfaces.Web;

namespace KrisG.Utility.Web
{
    public class WebStreamProvider : IWebStreamProvider
    {
        public Stream GetStream(string url)
        {
            var webRequest = WebRequest.Create(url) as HttpWebRequest;
            webRequest.AutomaticDecompression = DecompressionMethods.None | DecompressionMethods.Deflate | DecompressionMethods.GZip;
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.116 Safari/537.36";

            var webStream = webRequest.GetResponse().GetResponseStream();
            return webStream;
        }
    }
}