using KrisG.SickBeard.Client.Interfaces;
using Microsoft.Practices.Unity;

namespace KrisG.SickBeard.Client
{
    internal class ParserProvider : IParserProvider
    {
        private readonly IUnityContainer _container;

        public ParserProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IJsonDataParser<T> GetParser<T>()
        {
            return _container.Resolve<IJsonDataParser<T>>();
        }
    }
}