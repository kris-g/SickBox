using System;
using System.Reflection;

namespace KrisG.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ServiceInterfaceAttribute : Attribute
    {
        public string Name { get; private set; }

        public ServiceInterfaceAttribute(string name)
        {
            Name = name;
        }

        public static string GetName<T>()
        {
            var attribute = typeof(T).GetCustomAttribute<ServiceInterfaceAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException(string.Format("Type {0} missing expected {1} attribtue", typeof(T).Name, typeof(ServiceImplementationAttribute).Name));
            }

            return attribute.Name;
        }
    }
}