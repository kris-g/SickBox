using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class ShowDetailsParser : IJsonDataParser<ShowDetails>
    {
        public ShowDetails Parse(JObject data)
        {
            var mainItem = data["data"];
            if (mainItem != null)
            {
                var result = new ShowDetails(mainItem["location"].Value<string>());
                return result;
            }

            return null;
        }
    }
}