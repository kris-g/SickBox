using System.Collections.Generic;
using System.Xml.Linq;
using KrisG.KickassTorrents.Client.Data;

namespace KrisG.KickassTorrents.Client.Interfaces.Internal
{
    internal interface ISearchResultsParser
    {
        IEnumerable<SearchResult> Parse(XDocument doc);
    }
}