using System.Collections.Generic;
using System.IO;
using System.Net;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.Utility.Interfaces.Web;

namespace KrisG.IpTorrents.Client
{
    public class SearchClient : ISearchClient
    {
        private string _url;
        private string _username;
        private string _password;

        private readonly ISearchResultsParser _searchResultsParser;
        private readonly IFormAuthenticatedWebStreamProvider _webStreamProvider;

        public static ISearchClient Create(string url, string username, string password)
        {
            var client = new Container().Resolve<ISearchClient>();
            client.Initialise(url, username, password);

            return client;
        }

        internal SearchClient(
            ISearchResultsParser searchResultsParser,
            IFormAuthenticatedWebStreamProvider webStreamProvider)
        {
            _searchResultsParser = searchResultsParser;
            _webStreamProvider = webStreamProvider;
        }

        public void Initialise(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;

            _webStreamProvider.Username = _username;
            _webStreamProvider.Password = _password;
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            var url = BuildQueryUrl(query);

            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var response = reader.ReadToEnd();

            return _searchResultsParser.Parse(response);
        }

        public void DownloadTorrent(SearchResult item, string downloadPath)
        {
            var downloadUrl = string.Format(@"{0}{1}", _url, item.Link);

            var downloadFolder = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(downloadFolder);

            _webStreamProvider.DownloadFile(downloadUrl, downloadPath);
        }

        private string BuildQueryUrl(string query)
        {
            var encodedQuery = WebUtility.UrlEncode(query);
            var result = string.Format(@"{0}/t?q={1}", _url, encodedQuery);
            return result;
        }
    }
}
