namespace KrisG.SickBeard.Client.Interfaces
{
    public interface IParserProvider
    {
        IJsonDataParser<T> GetParser<T>();
    }
}