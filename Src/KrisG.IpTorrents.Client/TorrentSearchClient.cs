using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client
{
    public class TorrentSearchClient : ITorrentSearchClient
    {
        private const string DefaultBaseUrl = "https://iptorrents.eu";
        private const string LoginUrl = "take_login.php";

        private string _baseUrl;
        private string _username;
        private string _password;
        private IProxyConfig _proxyConfig;

        private readonly ISearchResultsParser _searchResultsParser;
        private readonly IFormAuthenticatedWebStreamProvider _webStreamProvider;

        public static ITorrentSearchClient Create(string username, string password, IProxyConfig proxyConfig = null)
        {
            return Create(DefaultBaseUrl, username, password, proxyConfig);
        }

        public static ITorrentSearchClient Create(string url, string username, string password, IProxyConfig proxyConfig = null)
        {
            var client = new Container().Resolve<ITorrentSearchClient>();
            client.Initialise(url, username, password, proxyConfig);

            return client;
        }

        internal TorrentSearchClient(
            ISearchResultsParser searchResultsParser,
            IFormAuthenticatedWebStreamProvider webStreamProvider)
        {
            _searchResultsParser = searchResultsParser;
            _webStreamProvider = webStreamProvider;
        }

        public void Initialise(string url, string username, string password, IProxyConfig proxyConfig)
        {
            _baseUrl = url;
            _username = username;
            _password = password;
            _proxyConfig = proxyConfig;

            _webStreamProvider.Username = _username;
            _webStreamProvider.Password = _password;
            _webStreamProvider.FormAuthUrl = $"{_baseUrl}/{LoginUrl}";
            _webStreamProvider.ProxyConfig = _proxyConfig;
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            var queryUrl = BuildQueryUrl(query);

            Process.Start(queryUrl);

            return Enumerable.Empty<SearchResult>();

            var stream = _webStreamProvider.GetStream(queryUrl);
            var reader = new StreamReader(stream);
            var response = reader.ReadToEnd();

            return _searchResultsParser.Parse(response);
        }

        public void DownloadTorrent(SearchResult item, string downloadPath)
        {
            var downloadUrl = $@"{_baseUrl}{item.Link}";

            var downloadFolder = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(downloadFolder);

            _webStreamProvider.DownloadFile(downloadUrl, downloadPath);
        }

        private string BuildQueryUrl(string query)
        {
            var encodedQuery = WebUtility.UrlEncode(query);
            var result = $@"{_baseUrl}/t?q={encodedQuery}&qf=all";
            return result;
        }
    }
}
