using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Enums;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class HistoryParser : IJsonDataParser<IEnumerable<HistoryEntry>>
    {
        public IEnumerable<HistoryEntry> Parse(JObject data)
        {
            if (data["data"] != null)
            {
                var parsed = data["data"]
                    .Children()
                    .Select(x => new HistoryEntry(
                        x["tvdbid"].Value<int>(),
                        x["show_name"].Value<string>(),
                        x["season"].Value<int>(),
                        x["episode"].Value<int>(),
                        x["date"].Value<DateTime>(),
                        x["quality"].Value<string>(),
                        (EpisodeStatus) Enum.Parse(typeof(EpisodeStatus), x["status"].Value<string>()
                        )))
                    .ToArray();

                return parsed;
            }

            return Enumerable.Empty<HistoryEntry>();
        }
    }
}