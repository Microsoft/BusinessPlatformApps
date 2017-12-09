using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedditCore;
using RedditCore.DataModel;
using RedditCore.Http;
using RedditCore.Logging;
using RedditCore.SocialGist;
using RedditCore.SocialGist.Model;
using RedditCore.Telemetry;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class SocialGistTest
    {
        private const string SOCIALGIST_KEY = "mykeyhere";

        private Mock<IApiPaginator> paginator;

        private RedditCore.SocialGist.SocialGist socialGist;

        [TestInitialize]
        public void SetUp()
        {
            paginator = new Mock<IApiPaginator>(MockBehavior.Strict);

            var configMock = new Mock<IConfiguration>();
            configMock.SetupGet(x => x.SocialGistApiKey).Returns(SOCIALGIST_KEY);

            socialGist = new RedditCore.SocialGist.SocialGist(
                configMock.Object,
                new ConsoleLog(),
                paginator.Object,
                Mock.Of<IHttpClient>(),
                Mock.Of<ITelemetryClient>()
            );
            socialGist.ResultLimitPerPage = 3;
        }

        [TestMethod]
        public async Task SearchResultsFromQuery()
        {
            Dictionary<string, object> paramDict; paramDict = socialGist.GetBaseParameters();
            paramDict.Add("query", "frogs");
            paramDict.Add("dn", "reddit.com");
            paramDict.Add("sort_mode", "time_desc");
            paramDict.Add("keep_original", "true");
            paramDict.Add("group_mode", "thread");
            paramDict.Add("match_mode", "boolean");

            var expected = new List<SearchMatch>
            {
                ApiTestHelper.ThreadIdToMatch("url1"),
                ApiTestHelper.ThreadIdToMatch("url1")
            };

            paginator.Setup(x => x.PageThroughCallResults<SearchApiResponse, SearchResponse, SearchMatches, SearchMatch>(
                "https://redditapi.socialgist.com/v1/Boards/Search",
                paramDict,
                socialGist.ResultLimitPerPage
            )).ReturnsAsync(expected);

            var threadIdSet = await socialGist.MatchesForQuery("frogs", null);

            CollectionAssert.AreEquivalent(threadIdSet, new SortedSet<SocialGistPostId>()
            {
                new SocialGistPostId()
                {
                    Url = "url1"
                }
            });
        }
    }
}
