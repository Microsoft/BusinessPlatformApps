using System.Collections.Generic;
using RedditCore.SocialGist.Model;

namespace RedditCoreTest.SocialGist.Mocks
{
    public class MockMatch : IMatch
    {
        public string Author { get; set; }
    }

    public class MockMatches : IMatches<MockMatch>
    {
        public List<MockMatch> Match { get; set; }
    }

    public class MockResponse : IResponse<MockMatches, MockMatch>
    {
        public Error Error { get; set; }
        public MockMatches Matches { get; set; }
        public string Total { get; set; }
    }

    public class MockApiResponse : IApiResponse<MockResponse, MockMatches, MockMatch>
    {
        public MockResponse response { get; set; }
    }
}
