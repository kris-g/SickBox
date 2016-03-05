using System;
using System.Collections.Generic;
using System.Linq;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using KrisG.Utility.Interfaces.Service;
using log4net;

namespace KrisG.Utility.Service
{
    public class ConfigurationServiceProvider : Interfaces.Service.IServiceProvider
    {
        private readonly IConfigurationReader _configReader;
        private readonly IServiceResolver _serviceResolver;
        private readonly ILog _log;

        public ConfigurationServiceProvider(IConfigurationReader configReader, IServiceResolver serviceResolver, ILog log)
        {
            _configReader = configReader;
            _serviceResolver = serviceResolver;
            _log = log;
        }

        public TService Get<TService>(bool optionalService = false)
        {
            return GetAll<TService>(optionalService).FirstOrDefault();
        }

        public IEnumerable<TService> GetAll<TService>(bool optionalService = false)
        {
            var name = ServiceInterfaceAttribute.GetName<TService>();

            var doc = _configReader.Read();
            var main = doc.Root.Element("Services").Element(name);

            if (main == null)
            {
                var message = string.Format("Failed to find service configuration for {0}", name);

                if (optionalService)
                {
                    _log.DebugFormat(message);
                    yield break;
                }
                
                throw new InvalidOperationException(message);
            }

            var items = main.Elements("Service").ToArray();

            if (items.Length == 0)
            {
                var message = string.Format("Failed to find service definition configurations for {0}", name);

                if (optionalService)
                {
                    _log.DebugFormat(message);
                    yield break;
                }
                
                throw new InvalidOperationException(message);
            }

            foreach (var element in items)
            {
                var implementationName = element.Attribute("Name").Value;
                var service = _serviceResolver.Get<TService>(implementationName, element);
               
                yield return service;
            }
        }
    }
}