using System.IO;
using System.Linq;
using System.Xml.Linq;
using KrisG.KickassTorrents.Client.Internal;
using NUnit.Framework;

namespace KrisG.KickassTorrents.Client.Test
{
    public class SearchResultsParserTests
    {
        [Test]
        public void ShouldParseSingleSearchResult()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("KrisG.KickassTorrents.Client.Test.TestData.SingleSearchResult.html");

            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();
            var doc = XDocument.Parse(testData);

            // act
            var results = new SearchResultsParser().Parse(doc).ToArray();

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
                .GetManifestResourceStream("KrisG.KickassTorrents.Client.Test.TestData.EmptySearchResult.html");

            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();
            var doc = XDocument.Parse(testData);

            // act
            var results = new SearchResultsParser().Parse(doc).ToArray();

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Length);
        }
    }
}
