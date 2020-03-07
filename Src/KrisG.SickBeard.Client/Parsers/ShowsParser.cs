using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Enums;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class ShowsParser : IJsonDataParser<IEnumerable<Show>>
    {
        public IEnumerable<Show> Parse(JObject data)
        {
            if (data["data"] != null)
            {
                var parsed = data["data"]
                    .Children()
                    .OfType<JProperty>()
                    .Select(x => new { Id = int.Parse(x.Name), Values = x.Value })
                    .Select(x => new Show(
                        x.Id,
                        x.Values["air_by_date"].Value<bool>(),
                        x.Values["language"].Value<string>(),
                        x.Values["network"].Value<string>(),
                        string.IsNullOrWhiteSpace(x.Values["next_ep_airdate"].Value<string>()) ? null : x.Values["next_ep_airdate"].Value<DateTime?>(),
                        x.Values["paused"].Value<bool>(),
                        x.Values["quality"].Value<string>(),
                        x.Values["show_name"].Value<string>(),
                        (ShowStatus)Enum.Parse(typeof(ShowStatus), x.Values["status"].Value<string>()),
                        x.Values["tvrage_id"]?.Value<int>() ?? 0,
                        x.Values["tvrage_name"]?.Value<string>()
                        ))
                    .ToArray();

                return parsed;
            }

            return Enumerable.Empty<Show>();
        }


    }
}