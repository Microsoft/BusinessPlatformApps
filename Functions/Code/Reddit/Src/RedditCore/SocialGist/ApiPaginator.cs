using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RedditCore.Http;
using RedditCore.Logging;
using RedditCore.SocialGist.Model;
using System.Text;
using RedditCore.Telemetry;

namespace RedditCore.SocialGist
{
    public class ApiPaginator: IApiPaginator
    {
        private readonly IHttpClient client;
        private readonly ILog log;
        private readonly IConfiguration configuration;
        private readonly ITelemetryClient telemetryClient;

        public ApiPaginator(
            IHttpClient client,
            ILog log, 
            ITelemetryClient telemetryClient,
            IConfiguration configuration)
        {
            this.log = log;
            this.configuration = configuration;
            this.client = client;
            this.telemetryClient = telemetryClient;
        }

        public async Task<List<TMatch>> PageThroughCallResults<TApiResponse, TResponse, TMatches, TMatch>(
            string baseUrl, 
            Dictionary<string, object> parameters, 
            int resultLimitPerPage
        ) 
            where TApiResponse : class, IApiResponse<TResponse, TMatches, TMatch> 
            where TResponse : IResponse<TMatches, TMatch>
            where TMatches : IMatches<TMatch>
            where TMatch : IMatch
        {
            var offset = 0;
            parameters.Add("offset", offset);
            parameters.Add("limit", resultLimitPerPage);

            var finalList = new List<TMatch>();

            var apiResponseTotal = 0;
            var requestTimeout = this.configuration.SocialGistApiRequestTimeout;

            do
            {
                log.Info("Seeking a new page");
                parameters["offset"] = offset;
                var uriBuilder = new UriBuilder(baseUrl);
                uriBuilder.QueryParamsFromDictionary(parameters);

                var result = await client.GetJsonAsync<TApiResponse>(uriBuilder.Uri, requestTimeout);
                log.Info("Page returned");

                await CheckError<TApiResponse, TResponse, TMatches, TMatch>(result);

                var resultList = result.Object.response.Matches.Match;
                log.Info("Page parsed into matches");
                
                finalList.AddRange(resultList);
                offset = finalList.Count;
                if (!int.TryParse(result.Object.response.Total, out apiResponseTotal))
                {
                    var errorMessage = $"For some reason, the Total number of results was either not returned or not a number from SocialGist!  Returned: [{result.Object.response.Total}]";
                    log.Error(errorMessage);
                    throw new ApiPaginatorException(errorMessage);
                }
                    

            } while (finalList.Count < apiResponseTotal);

            return finalList;
        }

        private async Task CheckError<TApiResponse, TResponse, TMatches, TMatch>(IHttpJsonResponseMessage<TApiResponse> response)
            where TApiResponse : class, IApiResponse<TResponse, TMatches, TMatch>
            where TResponse : IResponse<TMatches, TMatch>
            where TMatches : IMatches<TMatch>
            where TMatch : IMatch
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
            if (response?.Object?.response?.Error != null)
            {
                var errorMessage = $"Error retrieving comments: {response.Object.response.Error.ErrorMsg} ErrorCode: {response.Object.response.Error.ErrorCode}";
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
    }
}