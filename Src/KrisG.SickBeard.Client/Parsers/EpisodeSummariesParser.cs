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
                    .OfType<JProperty>()
                    .Select(x => new { EpisodeNumber = int.Parse(x.Name), Values = x.Value })
                    .Select(x => new EpisodeSummary(
                        x.EpisodeNumber,
                        x.Values["name"].Value<string>(),
                        AirDateIsValid(x.Values["airdate"].Value<string>()) ? x.Values["airdate"].Value<DateTime?>() : null,
                        x.Values["quality"].Value<string>(),
                        (EpisodeStatus) Enum.Parse(typeof(EpisodeStatus), x.Values["status"].Value<string>())
                        ))
                    .ToArray();

                return parsed;
            }

            return Enumerable.Empty<EpisodeSummary>();
        }

        private bool AirDateIsValid(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && !value.Equals("never", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}