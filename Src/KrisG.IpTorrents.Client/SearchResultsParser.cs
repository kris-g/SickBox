using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using CsQuery;
using KrisG.IpTorrents.Client.Data;
using KrisG.IpTorrents.Client.Interfaces.Internal;

namespace KrisG.IpTorrents.Client
{
    public class SearchResultsParser : ISearchResultsParser
    {
        public IEnumerable<SearchResult> Parse(string htmlContent)
        {
            var startIndex = htmlContent.IndexOf(@"<table id=torrents");
            var endIndex = htmlContent.IndexOf(@"</table>", startIndex);

            var mainTableContent = htmlContent.Substring(startIndex, endIndex - startIndex);

            if (mainTableContent.Contains("No Torrents Found"))
            {
                return Enumerable.Empty<SearchResult>();
            }

            var cq = CQ.CreateFragment(mainTableContent);
            var cq1 = cq["th"];

            var strings = cq1.Select(x => new { Item = x, Name = x.InnerText }).ToArray();
            var temp = strings
                .Select(x => string.IsNullOrEmpty(x.Name) ? ExtractColumnName(x.Item) : x.Name)
                .Select((x, idx) => new { Index = idx, Value = x })
                .ToArray();

            var colLookup = temp
                .ToDictionary(x => x.Value, x => x.Index);

            var rows = cq["tr"]
                .Select(x => CQ.CreateFragment(x.OuterHTML)["td"])
                .Where(x => x.Any())
                .Select(x => new SearchResult(
                    ParseLinkName(x[colLookup["Name"]]),
                    FileSize.Parse(x[colLookup["Torrent Size"]].InnerHTML),
                    Convert.ToInt32(x[colLookup["Snatches"]].InnerText),
                    Convert.ToInt32(x[colLookup["Sort by Seeders"]].InnerText),
                    Convert.ToInt32(x[colLookup["Sort by Leechers"]].InnerText),
                    ParseLink(x[colLookup["Download"]]),
                    ParseAlt(x[colLookup["Type"]])
                ))
                .ToArray();

            return rows;
        }

        private string ExtractColumnName(IDomObject dom)
        {
            var fragment = CQ.CreateFragment(dom.OuterHTML);

            var iconElement = fragment["i"];
            var linkElement = fragment["a"];


            if (iconElement.Elements.Any())
            {
                return fragment["i"].Attr<string>("title");
            }
            else if (linkElement.Elements.Any())
            {
                return fragment["a"].Text();
            }
            else
            {
                throw new InvalidDataException("Unexpected column header html");
            }
        }

        private string ParseAlt(IDomObject obj)
        {
            return CQ.CreateFragment(obj.InnerHTML)["img"].Attr("alt");
        }

        private string ParseLinkName(IDomObject obj)
        {
            return CQ.CreateFragment(obj.InnerHTML)["a"].Get().First().InnerText;
        }

        private string ParseLink(IDomObject obj)
        {
            return CQ.CreateFragment(obj.InnerHTML)["a"].Attr("href");
        }
    }
}