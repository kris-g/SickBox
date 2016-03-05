using KrisG.SickBox.Core.Interfaces.Enums;
using KrisG.SickBox.Core.Interfaces.Service;
using KrisG.Utility.Interfaces.Configuration;
using KrisG.Utility.Interfaces.Service;
using log4net;

namespace KrisG.SickBox.Core.Service
{
    public class ConfigurationServerProvider : IServerProvider
    {
        private readonly IConfigurationReader _configReader;
        private readonly IConfigResolver _configResolver;
        private readonly ILog _log;

        public ConfigurationServerProvider(IConfigurationReader configReader, IConfigResolver configResolver, ILog log)
        {
            _configReader = configReader;
            _configResolver = configResolver;
            _log = log;
        }

        public TConfig Get<TConfig>(ServerType type)
        {
            var doc = _configReader.Read();
            var main = doc.Root.Element("Servers");
            var item = main.Element(type.ToString());

            var config = _configResolver.Get<TConfig>(item);
            return config;
        }
    }
}