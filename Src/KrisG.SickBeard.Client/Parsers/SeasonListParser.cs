using System.Collections.Generic;
using System.Linq;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class SeasonListParser : IJsonDataParser<IEnumerable<int>>
    {
        public IEnumerable<int> Parse(JObject data)
        {
            var mainItem = data["data"];
            if (mainItem != null)
            {
                var result = mainItem.Values<int>();
                return result;
            }

            return Enumerable.Empty<int>();
        }
    }
}