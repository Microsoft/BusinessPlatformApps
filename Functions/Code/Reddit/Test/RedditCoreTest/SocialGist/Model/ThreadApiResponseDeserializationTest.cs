using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RedditCore.SocialGist.Model;
using RedditCoreTest.Properties;

namespace RedditCoreTest.SocialGist.Model
{
    [TestClass]
    public class ThreadApiResponseDeserializationTest
    {
        /// <summary>
        ///     A basic test to ensure that the response can be deserialized without error.  This does not mean that the response
        ///     is correct.
        /// </summary>
        [TestMethod]
        public void DeserializationSucceeds()
        {
            var result =
                JsonConvert.DeserializeObject<ThreadApiResponse>(Resources.ThreadApiResponse_NoErrors_5Results);

            Assert.IsNotNull(result, "Response not deserialized");
        }

        [TestMethod]
        public void DeserializeBigData()
        {
            var result =
                JsonConvert.DeserializeObject<ThreadApiResponse>(Resources.ThreadApiResponse_BigDataset);

            Assert.IsNotNull(result, "Response not deserialized");
        }

        [TestMethod]
        public void NoErrorsFileIsCorrect()
        {
            var result =
                JsonConvert.DeserializeObject<ThreadApiResponse>(Resources.ThreadApiResponse_NoErrors_5Results);

            Assert.IsNotNull(result, "Result not deserialized");

            // Check matches
            Assert.IsNotNull(result.Post, "Post should not be null");
            Assert.IsNotNull(result.Comments, "Comments should not be null");
            Assert.AreEqual(5, result.Comments.Count);

            // Match 1
            var comment1 = result.Comments[0];
            Assert.AreEqual("t5_2qh3s", comment1.SubredditId);
            Assert.AreEqual(1510363411, comment1.CreatedUtc);
            StringAssert.Contains(comment1.Content, "I'm in the minority here. I have never read the Dune series but have been a huge fan of Craig's Bond. He'd have made a great movie, I'm sure, because literally all of his films have been awesome. (Haven't seen his first 3 films)");
            Assert.AreEqual(1.0, result.Comments[0].Controversiality);

            // Match 2
            Assert.AreEqual(2.3, result.Comments[1].Controversiality);

            // Match 3
            Assert.AreEqual(0.0, result.Comments[2].Controversiality);
        }
    }
}
