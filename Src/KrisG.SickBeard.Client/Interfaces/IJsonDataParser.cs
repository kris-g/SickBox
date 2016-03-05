using Newtonsoft.Json.Linq;

namespace KrisG.SickBeard.Client.Interfaces
{
    public interface IJsonDataParser<T>
    {
        T Parse(JObject data);
    }
}