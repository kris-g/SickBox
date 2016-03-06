using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using KrisG.KickassTorrents.Client.Data;
using KrisG.KickassTorrents.Client.Interfaces.Internal;

namespace KrisG.KickassTorrents.Client.Internal
{
    public class SearchResultsParser : ISearchResultsParser
    {
        private readonly XNamespace _ns = @"//kastatic.com/xmlns/0.1/";

        public IEnumerable<SearchResult> Parse(XDocument doc)
        {
            return doc.Root
                .Element("channel")
                .Elements("item")
                .Select(x => new SearchResult(
                    x.Element("title").Value,
                    new FileSize(ValueAsLong(x, "contentLength")),
                    ValueAsInt(x, "seeds"),
                    ValueAsInt(x, "peers"),
                    x.Element(_ns + "fileName").Value,
                    x.Element("enclosure").Attribute("url").Value,
                    x.Element("category").Value
                    ));
        }

        private int ValueAsInt(XElement element, string name)
        {
            var valueElement = element.Element(_ns + name);
            var value = valueElement.Value;
            return int.Parse(value);
        }

        private long ValueAsLong(XElement element, string name)
        {
            var valueElement = element.Element(_ns + name);
            var value = valueElement.Value;
            return long.Parse(value);
        }
    }
}