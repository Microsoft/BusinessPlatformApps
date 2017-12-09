using System.Collections.Generic;
using System.Threading.Tasks;
using Ninject;
using RedditCore.DataModel;
using RedditCore.Logging;
using RedditCore.SocialGist.Model;
using RedditCore.Properties;
using System;
using System.Text;
using RedditCore.Http;
using RedditCore.Telemetry;

namespace RedditCore.SocialGist
{
    internal class SocialGist : ISocialGist
    {

        private readonly ILog log;
        private readonly IApiPaginator paginator;
        private readonly ITelemetryClient telemetryClient;
        private readonly IHttpClient httpClient;
        private readonly IConfiguration configuration;

        [Inject]
        public SocialGist(
            IConfiguration configuration,
            ILog log,
            IApiPaginator paginator,
            IHttpClient httpClient,
            ITelemetryClient telemetryClient
        )
        {
            this.log = log;
            this.configuration = configuration;
            this.paginator = paginator;
            this.telemetryClient = telemetryClient;
            this.httpClient = httpClient;
        }

        public int ResultLimitPerPage { get; set; } = 100;

        public int MaximumResultsPerSearch { get; set; } = 10000;

        public async Task<ThreadApiResponse> CommentsForThread(SocialGistPostId post)
        {
            // Telemetry
            telemetryClient.TrackEvent(TelemetryNames.SocialGist_Thread,
                new Dictionary<string, string> {{"url", post.Url}}
            );

            // Do the real work
            var parameters = GetBaseParameters();
            parameters.Add("url", post.Url);
            parameters.Add("use_compression", true);

            var uriBuilder = new UriBuilder("http://redditcomments.socialgist.com/");
            uriBuilder.QueryParamsFromDictionary(parameters);

            var result =
                await httpClient.GetJsonAsync<ThreadApiResponse>(uriBuilder.Uri,
                    configuration.SocialGistApiRequestTimeout);

            await CheckError(result);

            return result.Object;
        }

        private async Task CheckError(IHttpJsonResponseMessage<ThreadApiResponse> response)
        {
            var anyError = false;
            var errors = new StringBuilder();
            if (!response.ResponseMessage.IsSuccessStatusCode)
            {
                var error = await response.ResponseMessage.Content.ReadAsStringAsync();
                var errorString = $"Error returned from thread web service: {error}.  Response code: {response.ResponseMessage.StatusCode}";
                log.Error(errorString);
                errors.AppendLine(errorString);
                anyError = true;
            }
            if (response?.Object == null)
            {
                var errorString = "The API returned a successful response, but it also did not return any data.";
                log.Error(errorString);
                errors.AppendLine(errorString);
                anyError = true;
            }
            if (response?.Object?.RetryFatals != 0)
            {
                var errorMessage = $"Retry Fatals: {response?.Object?.RetryFatals}";
                log.Error(errorMessage);
                errors.AppendLine(errorMessage);
                anyError = true;
            }
            if (anyError)
            {
                this.telemetryClient.TrackEvent("An error occurred in this request",
                    new Dictionary<string, string>() {
                        { "Errors", errors.ToString() }
                    });
                throw new ApiPaginatorResponseException(errors.ToString());
            }
        }

        public async Task<SortedSet<SocialGistPostId>> MatchesForQuery(
            string query,
            long? startUnixTime = null
        )
        {
            telemetryClient.TrackEvent(TelemetryNames.SocialGist_Search, new Dictionary<string, string> { { "query", query } });

            var parameters = GetBaseParameters();
            parameters.Add("query", query);
            parameters.Add("dn", "reddit.com");
            parameters.Add("sort_mode", "time_desc");
            parameters.Add("keep_original", "true");
            parameters.Add("group_mode", "thread");
            parameters.Add("match_mode", "boolean");

            var threadIdSet = new SortedSet<SocialGistPostId>();

            var resultList = await paginator.PageThroughCallResults<SearchApiResponse, SearchResponse, SearchMatches, SearchMatch>(
                "https://redditapi.socialgist.com/v1/Boards/Search",
                parameters,
                ResultLimitPerPage
            );
            log.Verbose($"Returned Socialgist matches from query.  Count: {resultList.Count}");
            foreach (var match in resultList)
            {
                var postId = new SocialGistPostId()
                {
                    Url = match.Url
                };
                if (!threadIdSet.Contains(postId))
                {
                    log.Verbose($"--Adding {postId} to process queue");
                    threadIdSet.Add(postId);
                }
                else
                {
                    log.Verbose($"--Ignoring {postId} (already on process queue)");
                }
            }
            
            log.Info($"SocialGist query returned {threadIdSet.Count} unique results");
            return threadIdSet;
        }

        internal Dictionary<string, object> GetBaseParameters()
        {
            var dict = new Dictionary<string, object>
            {
                { "key", configuration.SocialGistApiKey },
                { "rt", "json" },
                { "max_matches", MaximumResultsPerSearch }
            };
            return dict;
        }
    }

}