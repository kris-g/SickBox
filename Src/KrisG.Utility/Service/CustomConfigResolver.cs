using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Castle.Components.DictionaryAdapter;
using KrisG.Utility.Attributes;
using KrisG.Utility.Interfaces.Service;
using log4net;

namespace KrisG.Utility.Service
{
    public class CustomConfigResolver : IConfigResolver
    {
        private readonly ILog _log;

        private static readonly DictionaryAdapterFactory _adapterFactory = new DictionaryAdapterFactory();
        
        public CustomConfigResolver(ILog log)
        {
            _log = log;
        }

        public TConfig Get<TConfig>(XElement element)
        {
            var configInstance = Get(typeof(TConfig), element);
            return (TConfig)configInstance;
        }

        public object Get(Type configType, XElement element)
        {
            var configIsInterface = configType.IsInterface;
            var propValues = new Hashtable();

            var propertyInfos = GetPublicProperties(configType);
            if (!configIsInterface)
            {
                propertyInfos = propertyInfos.Where(x => x.CanWrite).ToArray();
            }

            foreach (var info in propertyInfos)
            {
                var name = info.Name;

                try
                {
                    var propElement = element.Element(name);

                    if (propElement == null)
                    {
                        var requiredAttribute = info.GetCustomAttributes().OfType<RequiredAttribute>().FirstOrDefault();
                        if (requiredAttribute != null)
                        {
                            throw new InvalidOperationException(string.Format("Failed to parse config, required field missing [Service: {0}, Field: {1}]", configType.Name, name));
                        }

                        var defaultValueAttribute = info.GetCustomAttributes().OfType<DefaultValueAttribute>().FirstOrDefault();
                        if (defaultValueAttribute != null)
                        {
                            propValues[name] = defaultValueAttribute.Value;
                        }

                        // prevent optional items leaving null properties
                        if (info.PropertyType.IsArray)
                        {
                            propValues[name] = Array.CreateInstance(info.PropertyType.GetElementType(), 0);                            
                        }
                        else if (info.PropertyType == typeof(string))
                        {
                            propValues[name] = string.Empty;
                        }

                        continue;
                    }

                    var type = info.PropertyType;
                    var value = ParseValue(type, propElement);

                    propValues[name] = value;
                }
                catch (Exception ex)
                {
                    _log.WarnFormat("Failed to parse config field [Service: {0}, Field: {1}, Error: {2}]", configType.Name, name, ex.Message);
                }
            }

            var result = CreateInstance(configType, propValues);
            return result;
        }

        private object ParseValue(Type valueType, XElement element)
        {
            if (valueType.IsArray)
            {
                var arrayElements = element.Elements("Item");
                var arrayType = valueType.GetElementType();

                var objArray = arrayElements.Select(x => ParseValue(arrayType, x)).ToArray();
                var typedArray = Array.CreateInstance(arrayType, objArray.Length);

                objArray.CopyTo(typedArray, 0);

                return typedArray;
            }

            if (valueType.IsEnum)
            {
                var value = Enum.Parse(valueType, element.Value);
                return value;
            }

            if (valueType.IsValueType || valueType == typeof(string))
            {
                var value = Convert.ChangeType(element.Value, valueType);
                return value;
            }

            return Get(valueType, element);
        }

        private object CreateInstance(Type configType, IDictionary values)
        {
            object configInstance;
            
            if (configType.IsInterface)
            {
                configInstance = _adapterFactory.GetAdapter(configType, values);
            }
            else
            {
                configInstance = Activator.CreateInstance(configType);

                var propertyInfos = configType.GetProperties().Where(x => x.CanWrite);

                foreach (var info in propertyInfos)
                {
                    if (values.Contains(info.Name))
                    {
                        info.SetValue(configInstance, values[info.Name]);
                    }
                }
            }

            return configInstance;
        }

        public PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}