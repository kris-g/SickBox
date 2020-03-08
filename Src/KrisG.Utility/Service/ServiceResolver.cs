using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Configuration;
using KrisG.Utility.Interfaces.Service;
using System;
using System.Linq;
using System.Xml.Linq;
using Unity;

namespace KrisG.Utility.Service
{
    public class ServiceResolver : IServiceResolver
    {
        private readonly IConfigResolver _configResolver;
        private readonly IUnityContainer _container;

        public ServiceResolver(IConfigResolver configResolver, IUnityContainer container)
        {
            _configResolver = configResolver;
            _container = container;
        }

        public TService Get<TService>(string implementationName, XElement element)
        {
            var type = ServiceImplementationAttribute.GetImplementation<TService>(implementationName);
            var interfaces = type.GetInterfaces();
            var configInterface = interfaces
                .Where(x => x.IsGenericType)
                .FirstOrDefault(x => x.GetGenericTypeDefinition() == typeof(IConfigurable<>));

            var service = (TService) _container.Resolve(type);

            if (configInterface != null)
            {
                try
                {
                    var configType = configInterface.GetGenericArguments()[0];
                    var configInstance = _configResolver.Get(configType, element);

                    var serviceConfigProperty = service.GetType().GetProperties().First(x => x.PropertyType == configType);
                    serviceConfigProperty.SetValue(service, configInstance);
                }
                catch
                {
                    throw new InvalidOperationException(string.Format("Failed trying to build configuration [Name: {0}]{1}{2}", implementationName, Environment.NewLine, element));
                }
            }

            return service;
        }
    }
}