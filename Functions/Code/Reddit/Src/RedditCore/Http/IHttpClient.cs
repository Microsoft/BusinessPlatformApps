using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RedditCore.Http
{
    /// <summary>
    ///     Interface that wraps HTTP calls.  This is so that we can mock all HTTP calls for testing without having to use MSFT
    ///     Fakes.  This is also done so we can centralize serialization & deserialization of objects.
    /// </summary>
    public interface IHttpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri">URI to GET</param>
        /// <param name="requestTimeout">Timeout for the request.  If not specified then the timeout will be the default value defined by the .NET HTTP client.</param>
        /// <param name="authenticationHeaderValue">Authentication header.  If NULL then no authentication header will be used.</param>
        /// <returns></returns>
        Task<IHttpJsonResponseMessage<T>> GetJsonAsync<T>(
            Uri requestUri,
            TimeSpan? requestTimeout = null,
            AuthenticationHeaderValue authenticationHeaderValue = null);

        /// <summary>
        /// Issues an HTTP Delete.
        /// </summary>
        /// <param name="requestUri">URI to DELETE</param>
        /// <param name="requestTimeout">Timeout for the request.  If not specified then the timeout will be the default value defined by the .NET HTTP client.</param>
        /// <param name="authenticationHeaderValue">Authentication header.  If NULL then no authentication header will be used.</param>
        /// <param name="requestTimeout"></param>
        /// <returns></returns>
        Task<HttpResponseMessage> DeleteAsync(
            Uri requestUri,
            TimeSpan? requestTimeout = null,
            AuthenticationHeaderValue authenticationHeaderValue = null);
    }
}
