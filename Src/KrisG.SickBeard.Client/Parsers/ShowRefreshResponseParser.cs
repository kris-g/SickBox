using KrisG.SickBeard.Client.Data;
using KrisG.SickBeard.Client.Interfaces;
using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Parsers
{
    public class ShowRefreshResponseParser : IJsonDataParser<ShowRefreshResponse>
    {
        public ShowRefreshResponse Parse(JObject data)
        {
            return new ShowRefreshResponse(data["result"].Value<string>());
        }
    }
}