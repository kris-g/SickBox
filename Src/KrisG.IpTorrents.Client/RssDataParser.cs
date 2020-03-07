using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client
{
    public class RssDataParser : ISearchResultsParser
    {
        public IEnumerable<SearchResult> Parse(string content)
        {
            var doc = XDocument.Parse(content);

            return doc
                .Root
                ?.Elements().First()
                .Elements("item")
                .Select(e => new
                {
                    Name = e.Element("title")?.Value,
                    Link = e.Element("link")?.Value,
                    Description = e.Element("description")?.Value,
                    PublishedDate = DateTime.Parse(e.Element("pubDate")?.Value)
                })
                .Select(x => new SearchResult(x.Name, new FileSize(x.Description?.Split(';').First()), x.Link, x.Description?.Split(';').ElementAt(1).Trim(), x.PublishedDate ));
        }
    }
}