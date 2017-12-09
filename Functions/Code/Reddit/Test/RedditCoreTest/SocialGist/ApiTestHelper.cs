using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using RedditCore.Http;
using RedditCore.SocialGist.Model;
using RedditCoreTest.SocialGist.Mocks;

namespace RedditCoreTest.SocialGist
{
    public class ApiTestHelper
    {
        public static MockMatch AuthorToThreadMatch(string threadMatchAuthor)
        {
            return new MockMatch() { Author = threadMatchAuthor };
        }

        public static SearchMatch ThreadIdToMatch(
            string url
        )
        {
            var match = new SearchMatch
            {
                Url = url
            };
            return match;
        }

        public static IHttpJsonResponseMessage<MockApiResponse> CreateJsonThreadResponseMessage(List<MockMatch> matches, int total)
        {
            var r =
                new MockApiResponse { response = new MockResponse { Total = total.ToString(), Matches = new MockMatches { Match = matches } } };

            IHttpJsonResponseMessage<MockApiResponse> jsonResponseMessage =
                new HttpJsonResponseMessage<MockApiResponse>
                {
                    Object = r,
                    ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                };
            return jsonResponseMessage;
        }

        public static IHttpJsonResponseMessage<SearchApiResponse> CreateJsonSearchResponseMessage(List<SearchMatch> matches, int total)
        {
            var r =
                new SearchApiResponse { response = new SearchResponse { Total = total.ToString(), Matches = new SearchMatches { Match = matches } } };

            IHttpJsonResponseMessage<SearchApiResponse> jsonResponseMessage =
                new HttpJsonResponseMessage<SearchApiResponse>
                {
                    Object = r,
                    ResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                };
            return jsonResponseMessage;
        }
    }
}