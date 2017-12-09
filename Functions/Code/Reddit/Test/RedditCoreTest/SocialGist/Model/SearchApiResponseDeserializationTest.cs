using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RedditCore.SocialGist.Model;
using RedditCoreTest.Properties;

namespace RedditCoreTest.SocialGist.Model
{
    [TestClass]
    public class SearchApiResponseDeserializationTest
    {
        [TestMethod]
        public void DeserializeBigData()
        {
            var result =
                JsonConvert.DeserializeObject<SearchApiResponse>(Resources.SearchApiResponse_BigDataset);

            Assert.IsNotNull(result, "Response not deserialized");
        }
    }
}
