using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RedditCore;
using RedditCore.Http;
using RedditCore.Logging;
using RedditCore.SocialGist;
using RedditCore.SocialGist.Model;
using System.Net.Http.Headers;
using RedditCore.Telemetry;
using RedditCoreTest.SocialGist.Mocks;

namespace RedditCoreTest.SocialGist
{
    [TestClass]
    public class ApiPaginatorTest
    {
        private Mock<IHttpClient> httpClient;
        private IApiPaginator paginator;
        private ILog log;
        private int resultLimitPerPage = 3;
        private Dictionary<string, object> paramDict;
        private Mock<IConfiguration> configuration;

        [TestInitialize]
        public void SetUp()
        {
            httpClient = new Mock<IHttpClient>(MockBehavior.Strict);
            configuration = new Mock<IConfiguration>(MockBehavior.Strict);

            configuration.SetupGet(x => x.SocialGistApiRequestTimeout).Returns(TimeSpan.FromSeconds(65));

            log = new ConsoleLog();

            paginator = new ApiPaginator(
                httpClient.Object,
                log,
                Mock.Of<ITelemetryClient>(),
                configuration.Object
            );
            paramDict = new Dictionary<string, object>();
        }

        [TestMethod]
        public async Task PagingNotNeeded()
        {
            // set up expectated results
            var expectedMatches = new List<MockMatch>
            {
                ApiTestHelper.AuthorToThreadMatch("Author 1"),
                ApiTestHelper.AuthorToThreadMatch("Author 2")
            };
            var jsonResponseMessage = ApiTestHelper.CreateJsonThreadResponseMessage(expectedMatches, 2);

            // set up mocks and stubs
            httpClient.Setup(x => x.GetJsonAsync<MockApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Thread"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=0")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage));

            // apply method under test
            var actualMatches =
                await paginator.PageThroughCallResults<MockApiResponse, MockResponse, MockMatches, MockMatch>(
                    "http://api.boardreader.com/v1/Boards/Thread",
                    paramDict,
                    resultLimitPerPage
                );

            // validate
            CollectionAssert.AreEquivalent(expectedMatches, actualMatches);

            httpClient.VerifyAll();
        }

        [TestMethod]
        public async Task ExactlyOnePageReturnedWithResults()
        {
            // Setup the first response
            var expectedMatches = new List<MockMatch>
            {
                ApiTestHelper.AuthorToThreadMatch("Author 1"),
                ApiTestHelper.AuthorToThreadMatch("Author 2"),
                ApiTestHelper.AuthorToThreadMatch("Author 3")
            };
            var jsonResponseMessage1 = ApiTestHelper.CreateJsonThreadResponseMessage(expectedMatches, 3);

            httpClient.Setup(x => x.GetJsonAsync<MockApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Thread"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=0")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage1));


            // apply method under test
            var actualMatches =
                await paginator.PageThroughCallResults<MockApiResponse, MockResponse, MockMatches, MockMatch>(
                    "http://api.boardreader.com/v1/Boards/Thread",
                    paramDict,
                    resultLimitPerPage
                );

            httpClient.VerifyAll();

        }

        [TestMethod]
        public async Task OneFullPageOnePartialPage()
        {
            // Setup the first response
            var matches = new List<MockMatch>
            {
                ApiTestHelper.AuthorToThreadMatch("Author 1"),
                ApiTestHelper.AuthorToThreadMatch("Author 2"),
                ApiTestHelper.AuthorToThreadMatch("Author 3")
            };
            var jsonResponseMessage1 = ApiTestHelper.CreateJsonThreadResponseMessage(matches, 5);

            httpClient.Setup(x => x.GetJsonAsync<MockApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Thread"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=0")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage1));

            // Setup the second response
            var matches2 = new List<MockMatch>();
            matches2.Add(ApiTestHelper.AuthorToThreadMatch("Author 4"));
            matches2.Add(ApiTestHelper.AuthorToThreadMatch("Author 5"));
            var jsonResponseMessage2 = ApiTestHelper.CreateJsonThreadResponseMessage(matches2, 5);

            httpClient.Setup(x => x.GetJsonAsync<MockApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Thread"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=3")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage2));

            // apply method under test
            var actualMatches =
                await paginator.PageThroughCallResults<MockApiResponse, MockResponse, MockMatches, MockMatch>(
                    "http://api.boardreader.com/v1/Boards/Thread",
                    paramDict,
                    resultLimitPerPage
                );

            // Check Results
            var expected = new List<MockMatch>();
            expected.AddRange(matches);
            expected.AddRange(matches2);

            CollectionAssert.AreEquivalent(expected, actualMatches);

            httpClient.VerifyAll();
        }

        [TestMethod]
        public async Task PagingWithSearchResults()
        {
            var matches = new List<SearchMatch>
            {
                ApiTestHelper.ThreadIdToMatch("id3"),
                ApiTestHelper.ThreadIdToMatch("id3"),
                ApiTestHelper.ThreadIdToMatch("id2")
            };
            var jsonResponseMessage1 = ApiTestHelper.CreateJsonSearchResponseMessage(matches, 6);

            httpClient.Setup(x => x.GetJsonAsync<SearchApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Search"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=0")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage1));

            // Setup the second response
            var matches2 = new List<SearchMatch>
            {
                ApiTestHelper.ThreadIdToMatch("id1"),
                ApiTestHelper.ThreadIdToMatch("id4"),
                ApiTestHelper.ThreadIdToMatch("id2")
            };
            var jsonResponseMessage2 = ApiTestHelper.CreateJsonSearchResponseMessage(matches2, 6);

            httpClient.Setup(x => x.GetJsonAsync<SearchApiResponse>(It.Is<Uri>(y =>
                        y.AbsolutePath == "/v1/Boards/Search"
                        && y.Query.Contains("limit=3")
                        && y.Query.Contains("offset=3")),
                    It.Is<TimeSpan>(t => t == TimeSpan.FromSeconds(65)),
                    It.IsAny<AuthenticationHeaderValue>()))
                .Returns(Task.FromResult(jsonResponseMessage2));

            // apply method under test
            var actualMatches =
                await paginator.PageThroughCallResults<SearchApiResponse, SearchResponse, SearchMatches, SearchMatch>(
                    "https://redditapi.socialgist.com/v1/Boards/Search",
                    paramDict,
                    resultLimitPerPage
                );

            var expected = new List<SearchMatch>();
            expected.AddRange(matches);
            expected.AddRange(matches2);
            CollectionAssert.AreEquivalent(expected, actualMatches);

            httpClient.VerifyAll();
        }
    }
}
