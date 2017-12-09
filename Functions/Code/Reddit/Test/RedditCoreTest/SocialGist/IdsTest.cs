using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedditCore.Logging;
using RedditCore.SocialGist;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class IdsTest
    {

        private IIds ids;
        private Mock<ILog> logger;

        [TestInitialize]
        public void Setup()
        {
            logger = new Mock<ILog>(MockBehavior.Strict);
        }

        [TestMethod]
        public void IdFromUrl_null_url()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.IdFromUrl(null);
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void IdFromUrl_empty_url()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.IdFromUrl("");
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void IdFromUrl_good_value()
        {
            ids = new Ids(logger.Object);
            var returned = ids.IdFromUrl("http://google.com");
            Assert.AreEqual("aa2239c17609b21eba034c564af878f3eec8ce83ed0f2768597d2bc2fd4e4da5", returned,
                "Returned value didn't match the SHA256 generated via http://www.xorbin.com/tools/sha256-hash-calculator");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_null_parent_id()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl:
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/",
                childId: "dmo8j8n",
                parentId: null
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_null_child_url()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl: null,
                childId: "dmo8j8n",
                parentId: "dmjuw52"
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_null_child_id()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl:
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/",
                childId: null,
                parentId: "dmjuw52"
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_empty_parent_id()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl:
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/",
                childId: "dmo8j8n",
                parentId: ""
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_empty_child_url()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl: "",
                childId: "dmo8j8n",
                parentId: "dmjuw52"
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_empty_child_id()
        {
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl:
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/",
                childId: "",
                parentId: "dmjuw52"
            );
            Assert.IsNull(returned, "The returned value should have been null");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_expected_success()
        {
            ids = new Ids(logger.Object);
            const string parentUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmjuw52/";
            var returned = ids.ParentUrlFromChildDetails(
                childUrl:
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/",
                childId: "dmo8j8n",
                parentId: "dmjuw52"
            );
            Assert.AreEqual(parentUrl, returned, $"The generated URL {parentUrl} did not match {returned}");
            logger.Verify();
        }

        [TestMethod]
        public void ParentUrlFromChildDetails_unexpected_url()
        {
            const string childId = "dmo8j8n";
            const string parentId = "dmjuw52";
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ParentUrlFromChildDetails(
                childUrl: childUrl,
                childId: childId,
                parentId: parentId
            );
            Assert.IsNull(returned, $"Somehow an ID was generated: {returned}");
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_null_type_and_id()
        {
            const string typeAndId = null;
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_empty_type_and_id()
        {
            const string typeAndId = "";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_too_many_underscores()
        {
            const string typeAndId = "t1_t3_t4_dmjuw52";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_not_enough_underscores()
        {
            const string typeAndId = "dmjuw52";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_nothing_after_underscore()
        {
            const string typeAndId = "t1_";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_empty_after_splitting()
        {
            const string typeAndId = "t1_  ";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void ExtractIdFromTypeAndId_expected_success()
        {
            const string typeAndId = "t1_dmjuw52";
            const string expected = "dmjuw52";
            ids = new Ids(logger.Object);
            var returned = ids.ExtractIdFromTypeAndId(typeAndId);
            Assert.AreEqual(expected, returned, $"{expected} did not match {returned}");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_null_url()
        {
            const string childUrl = null;
            const string childId = "dmo8j8n";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_null_child_id()
        {
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/";
            const string childId = null;
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_empty_url()
        {
            const string childUrl = "";
            const string childId = "dmo8j8n";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_empty_child_id()
        {
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/";
            const string childId = "";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_child_id_not_in_url()
        {
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/";
            const string childId = "dmo8j8n";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.IsNull(returned, $"Expected a null value, somehow got a real one [{returned}]");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_child_id_trailing_slash()
        {
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n/";
            const string childId = "dmo8j8n";
            const string expected =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.AreEqual(expected, returned, $"Expected {expected} but got {returned}");
            logger.Verify();
        }

        [TestMethod]
        public void PostUrlFromChildDetails_child_id_no_trailing_slash()
        {
            const string childUrl =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/dmo8j8n";
            const string childId = "dmo8j8n";
            const string expected =
                "https://www.reddit.com/r/Competitiveoverwatch/comments/6xzuk2/doomfists_new_hitbox_analysis_xpost_from/";
            logger.Setup(x => x.Error(It.IsAny<string>(), null, null));
            ids = new Ids(logger.Object);
            var returned = ids.PostUrlFromChildDetails(childUrl, childId);
            Assert.AreEqual(expected, returned, $"Expected {expected} but got {returned}");
            logger.Verify();
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_is_comment()
        {
            const string url =
                "https://www.reddit.com/r/Hawaii/comments/6z7mcz/eli5_molokai_ranch_which_covers_about_35_of_the/dmt4nvx/";

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));
            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.AreEqual("Hawaii", result);
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_is_post()
        {
            const string url =
                "https://www.reddit.com/r/Hawaii/comments/6z7mcz/eli5_molokai_ranch_which_covers_about_35_of_the/";

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));
            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.AreEqual("Hawaii", result);
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_httpNotSecure()
        {
            const string url =
                "http://www.reddit.com/r/Hawaii/comments/6z7mcz/eli5_molokai_ranch_which_covers_about_35_of_the/";

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));
            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.AreEqual("Hawaii", result);
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_noWWW()
        {
            const string url =
                "https://reddit.com/r/Hawaii/comments/6z7mcz/eli5_molokai_ranch_which_covers_about_35_of_the/";

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));
            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.AreEqual("Hawaii", result);
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_is_null()
        {
            const string url = null;

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));

            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.IsNull(result);

            logger.Verify(
                x => x.Info(
                    It.Is<string>(message => message == "URL is null or empty.  No subreddit ID found."),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_is_empty()
        {
            var url = string.Empty;

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));

            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.IsNull(result);

            logger.Verify(
                x => x.Info(
                    It.Is<string>(message => message == "URL is null or empty.  No subreddit ID found."),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }

        [TestMethod]
        public void SubredditIdFromUrl_url_is_not_reddit()
        {
            const string url =
                "https://www.example.com/stuff";

            logger.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<string>()));

            ids = new Ids(logger.Object);
            var result = ids.ExtractSubredditIdFromUrl(url);

            Assert.IsNull(result);

            logger.Verify(
                x => x.Info(
                    It.Is<string>(message => message == $"No subreddit found in URL {url}"),
                    It.IsAny<string>()
                ),
                Times.Once
            );
        }
    }
}
