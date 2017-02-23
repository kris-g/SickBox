using System.IO;
using System.Linq;
using NUnit.Framework;

namespace KrisG.IpTorrents.Client.Test
{
    public class SearchResultsParserTests
    {
        [Test]
        public void ShouldParseRawResults()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("KrisG.IpTorrents.Client.Test.TestData.RawSearchResults1.html");
            
            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();

            // act
            var results = new SearchResultsParser().Parse(testData).ToArray();

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Length);
        }

        [Test]
        public void EmptyResultsShouldReturnNoResults()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("KrisG.IpTorrents.Client.Test.TestData.RawEmptySearchResults.html");
            
            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();

            // act
            var results = new SearchResultsParser().Parse(testData).ToArray();

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }
    }
}
