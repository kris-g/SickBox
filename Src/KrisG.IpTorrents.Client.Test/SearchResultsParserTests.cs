using System.IO;
using System.Linq;
using NUnit.Framework;

namespace KrisG.IpTorrents.Client.Test
{
    public class SearchResultsParserTests
    {
        [Test]
        public void ShouldParseResult()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("KrisG.IpTorrents.Client.Test.TestData.SearchResults1.html");
            
            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();

            // act
            var results = new SearchResultsParser().Parse(testData).ToArray();

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(50, results.Length);
        }
    }
}
