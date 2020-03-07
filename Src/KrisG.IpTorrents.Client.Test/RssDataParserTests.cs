using System.IO;
using System.Linq;
using NUnit.Framework;

namespace KrisG.IpTorrents.Client.Test
{
    public class RssDataParserTests
    {
        [Test]
        public void ShouldParseRssData()
        {
            // assemble
            var testDataStream = GetType()
                .Assembly
                .GetManifestResourceStream("KrisG.IpTorrents.Client.Test.TestData.RssData.xml");

            var reader = new StreamReader(testDataStream);
            var testData = reader.ReadToEnd();

            // act
            var results = new RssDataParser().Parse(testData).ToArray();

            // assert
            Assert.IsNotNull(results);
            Assert.AreEqual(100, results.Length);
        }
    }
}