using System.Collections.Generic;
using KrisG.IpTorrents.Client.Data;

namespace KrisG.IpTorrents.Client.Interfaces
{
    public interface ISearchResultsParser
    {
        IEnumerable<SearchResult> Parse(string htmlContent);
    }
}