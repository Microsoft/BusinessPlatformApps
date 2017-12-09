using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Common.Helpers
{
    public class RestClient
    {
        public readonly string ID;

        private readonly AuthenticationHeaderValue _authenticationInfo;
        private readonly string _baseUri;
        private readonly Dictionary<string, string> _headers;
        private readonly string _mediaType;

        public RestClient(string baseUri, AuthenticationHeaderValue authenticationInfo = null, Dictionary<string, string> headers = null, string id = null, string mediaType = "application/json")
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
            ServicePointManager.CheckCertificateRevocationList = true;
            _baseUri = baseUri;
            _headers = headers;
            _mediaType = mediaType;

            ID = id;

            // Set authorization
            if (authenticationInfo != null)
                _authenticationInfo = authenticationInfo;
        }

        private string SanitizeBody(string body)
        {
            return string.IsNullOrEmpty(body) ? string.Empty :
                                                Regex.Replace(body, "\\s*\"password\"\\s*:\\s*\".*?\"\\s*", "password\": \"XXXXXXX\"", RegexOptions.IgnoreCase);
        }

        public async Task<string> HandleRequest(HttpMethod method, string relativeUri, Dictionary<string, string> headers, string body)
        {
            string responseMessage;

            using (HttpClient client = new HttpClient() { BaseAddress = new Uri(_baseUri) })
            {
                client.DefaultRequestHeaders.Authorization = _authenticationInfo;

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_mediaType));

                HttpRequestMessage message = new HttpRequestMessage(method, relativeUri);

                if (headers != null)
                {
                    client.DefaultRequestHeaders.Clear();
                    headers.Keys.ToList().ForEach(p => client.DefaultRequestHeaders.Add(p, headers[p]));
                }

                if (body != null)
                {
                    message.Content = new StringContent(body, Encoding.UTF8, _mediaType);
                }

                var response = await client.SendAsync(message);
                responseMessage = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                    return responseMessage;
            }

            throw new HttpRequestException(responseMessage);
        }

        public async Task<string> Get(string relativeUri, string parameters = null, Dictionary<string, string> headers = null)
        {
            headers = headers ?? _headers;
            return parameters == null
                ? await HandleRequest(HttpMethod.Get, relativeUri, headers, null)
                : await HandleRequest(HttpMethod.Get, string.Concat(relativeUri, '?', parameters), headers, null);
        }

        public async Task<string> Post(string relativeUri, string body, string parameters = null, Dictionary<string, string> headers = null)
        {
            headers = headers ?? _headers;
            return parameters == null
                ? await HandleRequest(HttpMethod.Post, relativeUri, headers, body)
                : await HandleRequest(HttpMethod.Post, string.Concat(relativeUri, '?', parameters), headers, body);
        }

        public async Task<string> Put(string relativeUri, string body, Dictionary<string, string> headers = null)
        {
            headers = headers ?? _headers;
            return await HandleRequest(HttpMethod.Put, relativeUri, headers, body);
        }

        public async Task<string> Delete(string relativeUri, string parameters = null, Dictionary<string, string> headers = null)
        {
            headers = headers ?? _headers;
            return parameters == null
                ? await HandleRequest(HttpMethod.Delete, relativeUri, headers, null)
                : await HandleRequest(HttpMethod.Delete, string.Concat(relativeUri, '?', parameters), headers, null);
        }
    }
}