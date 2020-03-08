using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces.Internal;
using Shaman.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
                .Select(x => new SearchResult(x.Name, FileSize.Parse(x.Description?.Split(';').First()), x.Link, x.Description?.Split(';').ElementAt(1).Trim(), x.PublishedDate ));
        }
    }
}