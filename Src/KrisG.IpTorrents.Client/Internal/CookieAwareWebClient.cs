using System;
using System.Net;

namespace KrisG.IpTorrents.Client.Internal
{
    internal class CookieAwareWebClient : WebClient
    {
        public CookieAwareWebClient()
        {
            CookieContainer = new CookieContainer();
        }
        public CookieContainer CookieContainer { get; set; }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);

            request.CookieContainer = CookieContainer;

            return request;
        }
    }
}