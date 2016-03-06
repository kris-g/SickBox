using System.Collections.Generic;
using System.IO;
using System.Net;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client
{
    public class TorrentSearchClient : ITorrentSearchClient
    {
        private const string DefaultBaseUrl = "https://iptorrents.eu";

        private string _baseUrl;
        private string _username;
        private string _password;

        private readonly ISearchResultsParser _searchResultsParser;
        private readonly IFormAuthenticatedWebStreamProvider _webStreamProvider;

        public static ITorrentSearchClient Create(string username, string password)
        {
            return Create(DefaultBaseUrl, username, password);
        }

        public static ITorrentSearchClient Create(string url, string username, string password)
        {
            var client = new Container().Resolve<ITorrentSearchClient>();
            client.Initialise(url, username, password);

            return client;
        }

        internal TorrentSearchClient(
            ISearchResultsParser searchResultsParser,
            IFormAuthenticatedWebStreamProvider webStreamProvider)
        {
            _searchResultsParser = searchResultsParser;
            _webStreamProvider = webStreamProvider;
        }

        public void Initialise(string url, string username, string password)
        {
            _baseUrl = url;
            _username = username;
            _password = password;

            _webStreamProvider.Username = _username;
            _webStreamProvider.Password = _password;
            _webStreamProvider.FormAuthUrl = _baseUrl;
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            var queryUrl = BuildQueryUrl(query);

            var stream = _webStreamProvider.GetStream(queryUrl);
            var reader = new StreamReader(stream);
            var response = reader.ReadToEnd();

            return _searchResultsParser.Parse(response);
        }

        public void DownloadTorrent(SearchResult item, string downloadPath)
        {
            var downloadUrl = string.Format(@"{0}{1}", _baseUrl, item.Link);

            var downloadFolder = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(downloadFolder);

            _webStreamProvider.DownloadFile(downloadUrl, downloadPath);
        }

        private string BuildQueryUrl(string query)
        {
            var encodedQuery = WebUtility.UrlEncode(query);
            var result = string.Format(@"{0}/t?q={1}", _baseUrl, encodedQuery);
            return result;
        }
    }
}
