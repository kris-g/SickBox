using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace KrisG.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceImplementationAttribute : Attribute
    {
        public string Name { get; private set; }

        public ServiceImplementationAttribute(string name)
        {
            Name = name;
        }

        public static Type GetImplementation<TInterface>(string name)
        {
            var result = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(GetTypesSafe).Where(x => typeof(TInterface).IsAssignableFrom(x))
                .Where(x => !x.IsInterface)
                .Select(x => new { Type = x, Attr = x.GetCustomAttribute<ServiceImplementationAttribute>() })
                .Where(x => x.Attr != null)
                .Where(x => x.Attr.Name == name)
                .Select(x => x.Type)
                .FirstOrDefault();

            if (result == null)
            {
                throw new InvalidOperationException(string.Format("Failed to find service implementation {0} for interface {1}", name, typeof(TInterface).Name));
            }

            return result;
        }

        private static IEnumerable<Type> GetTypesSafe(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Enumerable.Empty<Type>();
            }
        }
    }
}