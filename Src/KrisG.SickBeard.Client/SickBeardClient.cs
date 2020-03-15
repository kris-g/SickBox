using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Interfaces;
using KrisG.Utility.Extensions;
using KrisG.Utility.Interfaces.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace KrisG.SickBeard.Client
{
    public class SickBeardClient : ISickBeardClient
    {
        private string _url;
        private string _apiKey;

        private readonly IWebStreamProvider _webStreamProvider;
        private readonly IParserProvider _parserProvider;

        public static ISickBeardClient Create(string url, string apiKey)
        {
            var client = new Container().Resolve<ISickBeardClient>();
            client.Initialise(url, apiKey);

            return client;
        }

        internal SickBeardClient(IWebStreamProvider webStreamProvider, IParserProvider parserProvider)
        {
            _webStreamProvider = webStreamProvider;
            _parserProvider = parserProvider;
        }

        public void Initialise(string url, string apiKey)
        {
            _url = url;
            _apiKey = apiKey;            
        }

        public IEnumerable<Show> Shows()
        {
            var url = BuildUrl("shows");
            
            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);

            var jsonObj = JObject.Load(jsonReader);

            var parser = _parserProvider.GetParser<IEnumerable<Show>>();
            var result = parser.Parse(jsonObj);

            return result;
        }

        public ShowDetails Show(int id)
        {
            var args = new[] { new Tuple<string, string>("tvdbid", id.ToString()) };
            var url = BuildUrl("show", args);

            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);

            var jsonObj = JObject.Load(jsonReader);

            var parser = _parserProvider.GetParser<ShowDetails>();
            var result = parser.Parse(jsonObj);

            return result;
        }

        public IEnumerable<int> ShowSeasonList(int id)
        {
            var args = new [] { new Tuple<string, string>("tvdbid", id.ToString()) };
            var url = BuildUrl("show.seasonlist", args);

            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);

            var jsonObj = JObject.Load(jsonReader);

            var parser = _parserProvider.GetParser<IEnumerable<int>>();
            var result = parser.Parse(jsonObj);

            return result;
        }

        public SeasonSummary ShowSeason(int id, int seasonNumber)
        {
            var args = new [] { new Tuple<string, string>("tvdbid", id.ToString()), new Tuple<string, string>("season", seasonNumber.ToString()) };
            var url = BuildUrl("show.seasons", args);

            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);

            var jsonObj = JObject.Load(jsonReader);

            var parser = _parserProvider.GetParser<IEnumerable<EpisodeSummary>>();
            var result = parser.Parse(jsonObj);

            return new SeasonSummary(id, seasonNumber, result);
        }

        public IEnumerable<HistoryEntry> History(int? limit)
        {
            var args = limit.HasValue ? new[] { new Tuple<string, string>("limit", limit.ToString()) } : new Tuple<string, string>[0];
            var url = BuildUrl("history", args);

            var stream = _webStreamProvider.GetStream(url);
            var reader = new StreamReader(stream);
            var jsonReader = new JsonTextReader(reader);

            var jsonObj = JObject.Load(jsonReader);

            var parser = _parserProvider.GetParser<IEnumerable<HistoryEntry>>();
            var result = parser.Parse(jsonObj);

            return result;
        }

        public IEnumerable<SeasonSummary> ShowSeasons(int id)
        {
            return null;
        }

        public bool ShowRefresh(int id)
        {
            var args = new[] { new Tuple<string, string>("tvdbid", id.ToString()) };
            var url = BuildUrl("show.refresh", args);

            using (var stream = _webStreamProvider.GetStream(url))
            {
                var reader = new StreamReader(stream);
                var jsonReader = new JsonTextReader(reader);

                var jsonObj = JObject.Load(jsonReader);

                var parser = _parserProvider.GetParser<ShowRefreshResponse>();
                var result = parser.Parse(jsonObj);

                return result.Result == "success";
            }
        }

        public void FixFileNames(int showId, IEnumerable<(int season, int episode)> episodes)
        {
            var client = new RestClient($"{_url}/home/doRename");
            var request = new RestRequest();

            request.AddParameter("show", showId.ToString());
            request.AddParameter("eps", episodes.Select(ep => $"{ep.season}x{ep.episode}").JoinStrings("|"));

            var response = client.Post(request);
            if (!response.IsSuccessful)
            {
                throw new InvalidOperationException("Request failed", response.ErrorException);
            }
        }

        private string BuildUrl(string cmd, ICollection<Tuple<string, string>> args = null)
        {
            if (args == null)
            {
                args = new Collection<Tuple<string, string>>();
            }

            return string.Format("{0}/api/{1}/?cmd={2}{3}{4}", _url, _apiKey, cmd, args.Count != 0 ? "&" : string.Empty, string.Join("&", args.Select(x => string.Format("{0}={1}", x.Item1, x.Item2))));
        }
    }
}
