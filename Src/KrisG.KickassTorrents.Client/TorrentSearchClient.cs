using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using KrisG.KickassTorrents.Client.Data;
using KrisG.KickassTorrents.Client.Interfaces;
using KrisG.KickassTorrents.Client.Interfaces.Internal;
using KrisG.Utility.Interfaces.Web;

namespace KrisG.KickassTorrents.Client
{
    public class TorrentSearchClient : ITorrentSearchClient
    {
        public const string DefaultBaseUrl = "https://kat.cr";
        
        private string _baseUrl;

        private readonly ISearchResultsParser _searchResultsParser;
        private readonly IWebStreamProvider _webStreamProvider;

        public static ITorrentSearchClient Create()
        {
            return Create(DefaultBaseUrl);
        }

        public static ITorrentSearchClient Create(string url)
        {
            var client = new Container().Resolve<ITorrentSearchClient>();
            client.Initialise(url);

            return client;
        }

        internal TorrentSearchClient(ISearchResultsParser searchResultsParser, IWebStreamProvider webStreamProvider)
        {
            _searchResultsParser = searchResultsParser;
            _webStreamProvider = webStreamProvider;
        }

        public void Initialise(string url)
        {
            _baseUrl = url;
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            var queryUrl = BuildQueryUrl(query);

            try
            {
                var stream = _webStreamProvider.GetStream(queryUrl);

                var reader = new StreamReader(stream);
                var response = reader.ReadToEnd();

                var doc = XDocument.Parse(response);

                return _searchResultsParser.Parse(doc);
            }
            catch (WebException)
            {
                return Enumerable.Empty<SearchResult>();
            }
        }

        public void DownloadTorrent(SearchResult item, string downloadPath)
        {
            var downloadFolder = Path.GetDirectoryName(downloadPath);
            Directory.CreateDirectory(downloadFolder);

            using (var readStream = _webStreamProvider.GetStream(item.Link))
            {
                using (var writeStream = File.OpenWrite(downloadPath))
                {
                    readStream.CopyTo(writeStream);
                }
            }
        }

        private string BuildQueryUrl(string query)
        {
            var encodedQuery = WebUtility.UrlEncode(query);
            var result = string.Format(@"{0}/usearch/{1}/?rss=1", _baseUrl, encodedQuery);
            return result;
        }
    }
}