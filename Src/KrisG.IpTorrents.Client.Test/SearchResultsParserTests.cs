using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KrisG.IpTorrents.Client.Test
{
    [TestClass]
    public class SearchResultsParserTests
    {
        [TestMethod]
        public void ShouldParseResult()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("IpTorrents.Client.Test.TestData.SearchResults1.html");
            
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
