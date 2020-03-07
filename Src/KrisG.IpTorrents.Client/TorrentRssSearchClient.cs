using System.Collections.Generic;
using System.IO;
using System.Linq;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces;
using KrisG.IpTorrents.Client.Interfaces.Internal;
using log4net;
using Newtonsoft.Json;

namespace KrisG.IpTorrents.Client
{
    public class TorrentRssSearchClient : ITorrentSearchClient
    {
        private const string _cacheFilePath = @".\IpTorrentRssDataCache.json";

        private IReadOnlyCollection<SearchResult> _searchResults;

        private readonly ISearchResultsParser _searchResultsParser;
        private readonly IWebStreamProvider _webStreamProvider;
        private readonly ILog _log;

        public static ITorrentSearchClient Create(string rssUrl)
        {
            var client = new Container().Resolve<TorrentRssSearchClient>();
            client.Initialise(rssUrl);

            return client;
        }

        internal TorrentRssSearchClient(ISearchResultsParser searchResultsParser, IWebStreamProvider webStreamProvider, ILog log)
        {
            _searchResultsParser = searchResultsParser;
            _webStreamProvider = webStreamProvider;
            _log = log;
        }

        public void Initialise(string rssUrl)
        {
            using (var stream = _webStreamProvider.GetStream(rssUrl))
            {
                var rssContent = new StreamReader(stream).ReadToEnd();
                _searchResults = _searchResultsParser.Parse(rssContent).ToArray();
            }

            var originalRssResultCount = _searchResults.Count;

            var cacheResults = LoadCache();

            _searchResults = _searchResults
                .Where(x => !cacheResults.Any(cr => cr.Link == x.Link))
                .Concat(cacheResults)
                .OrderByDescending(x => x.PublishedDate)
                .Take(20000)
                .ToArray();

            SaveCache(_searchResults);

            _log.Info($"RSS data loaded [RssDataCount: {originalRssResultCount}, CacheCount: {cacheResults.Count}, TotalCount: {_searchResults.Count}]");
        }

        public IEnumerable<SearchResult> Search(string query)
        {
            return _searchResults.Where(sr => query.ToLower().Split(' ').Where(qw => qw.Length > 1).All(qw => sr.Name.ToLower().Contains(qw)));
        }

        public void DownloadTorrent(SearchResult item, string downloadPath)
        {
            _webStreamProvider.DownloadFile(item.Link, downloadPath);
        }

        private IReadOnlyCollection<SearchResult> LoadCache()
        {
            if (!File.Exists(_cacheFilePath)) return Enumerable.Empty<SearchResult>().ToArray();

            return File
                .ReadLines(_cacheFilePath)
                .Select(l => JsonConvert.DeserializeObject<SearchResult>(l))
                .ToArray();
        }

        private void SaveCache(IEnumerable<SearchResult> items)
        {
            File.WriteAllLines(_cacheFilePath, items.Select(x => JsonConvert.SerializeObject(x)));
        }
    }
}