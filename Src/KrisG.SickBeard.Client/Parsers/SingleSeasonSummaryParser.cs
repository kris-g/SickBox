using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Enums;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class EpisodeSummariesParser : IJsonDataParser<IEnumerable<EpisodeSummary>>
    {
        public IEnumerable<EpisodeSummary> Parse(JObject data)
        {
            if (data["data"] != null)
            {
                var parsed = data["data"]
                    .Children()
                    .SelectMany(x => x.Children())
                    .SelectMany(x => x.Children())
                    .OfType<JProperty>()
                    .Select(x => new { EpisodeNumber = int.Parse(x.Name), Values = x.Value })
                    .Select(x => new EpisodeSummary(
                        x.EpisodeNumber,
                        x.Values["name"].Value<string>(),
                        string.IsNullOrWhiteSpace(x.Values["airdate"].Value<string>()) ? null : x.Values["airdate"].Value<DateTime?>(),
                        x.Values["quality"].Value<string>(),
                        (EpisodeStatus) Enum.Parse(typeof(EpisodeStatus), x.Values["status"].Value<string>())
                        ))
                    .ToArray();

                return parsed;
            }

            return Enumerable.Empty<EpisodeSummary>();
        }
    }
}