using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using KrisG.Utility.Interfaces.Service;

namespace KrisG.Utility.Service
{
    public class XmlSerializerConfigResolver : IConfigResolver
    {
        public TConfig Get<TConfig>(XElement element)
        {
            var configInstance = Get(typeof(TConfig), element);
            return (TConfig) configInstance;
        }

        public object Get(Type configType, XElement element)
        {
            var xmlReader = element.CreateReader();
            xmlReader.MoveToContent();

            var innerXml = xmlReader.ReadInnerXml();
            var rebuiltXml = string.Format("<{0}>{1}</{0}>", configType.Name, innerXml);

            //var arrayItemAttribute = new XmlArrayItemAttribute("Item", typeof(string));

            //var attributes = new XmlAttributes();
            ////attributes.XmlRoot = new XmlRootAttribute(configType.Name);
            ////attributes.XmlType = new XmlTypeAttribute(configType.Name);
            //attributes.XmlArray = new XmlArrayAttribute("SearchQueryAdditions");
            //attributes.XmlArrayItems.Add(arrayItemAttribute);

            var xao = new XmlAttributeOverrides();
            var propertyInfos = configType.GetProperties().Where(x => x.CanWrite);
            foreach (var info in propertyInfos)
            {
                if (info.PropertyType.IsArray)
                {
                    ApplyAttrOverrides(xao, configType, info.Name, info.PropertyType.GetElementType());
                }
            }

            var configInstance = new XmlSerializer(configType, xao).Deserialize(new StringReader(rebuiltXml));
            return configInstance;
        }


        private void ApplyAttrOverrides(XmlAttributeOverrides xao, Type configType, string name, Type getElementType)
        {
            var arrayItemAttribute = new XmlArrayItemAttribute("Item", getElementType);

            var attributes = new XmlAttributes();
            attributes.XmlArray = new XmlArrayAttribute(name);
            attributes.XmlArrayItems.Add(arrayItemAttribute);

            xao.Add(configType, name, attributes);
        }
    }
}