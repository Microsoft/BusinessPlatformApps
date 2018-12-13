using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.Model.Http;

namespace Microsoft.Deployment.Common.Helpers
{
    public class AzureHttpClient
    {
        private const int FORM_BOUNDARY_SIZE = 15;

        public Dictionary<string, string> Headers { get; set; }
        public Task Providers { get; set; }
        public string ResourceGroup { get; }
        public string Subscription { get; }
        public string Token { get; set; }

        public AzureHttpClient(string token)
        {
            this.Token = token;
        }

        public AzureHttpClient(string token, string subscriptionId)
        {
            this.Token = token;
            this.Subscription = subscriptionId;
        }

        public AzureHttpClient(string token, string subscription, string resourceGroup)
        {
            this.Token = token;
            this.Subscription = subscription;
            this.ResourceGroup = resourceGroup;
        }

        public AzureHttpClient(Dictionary<string, string> headers)
        {
            this.Headers = headers;
        }

        public async Task<HttpResponseMessage> ExecuteGenericRequestWithHeaderAsync(HttpMethod method, string url, string body)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = url;
                HttpRequestMessage message = new HttpRequestMessage(method, requestUri);

                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    message.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);

                if (this.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header in this.Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                return await client.SendAsync(message);
            }
        }

        public async Task<HttpResponseMessage> ExecuteGenericRequestWithHeaderAsync(HttpMethod method, string url, byte[] body)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = url;
                HttpRequestMessage message = new HttpRequestMessage(method, requestUri);

                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    message.Content = new ByteArrayContent(body);
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);

                if (this.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header in this.Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                return await client.SendAsync(message);
            }
        }

        public async Task<HttpResponseMessage> ExecuteWebsiteAsync(HttpMethod method, string site, string relativeUrl, string body)
        {
            string requestUri = $"https://{site}{Constants.AzureWebSite}{relativeUrl}";

            return await this.ExecuteGenericRequestWithHeaderAsync(method, requestUri, body);
        }

        public async Task<HttpResponseMessage> ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod method, string relativeUrl, string apiVersion, string body, Dictionary<string, string> queryParameters)
        {
            StringBuilder parameters = new StringBuilder();
            parameters.Append("?");
            foreach (var parameter in queryParameters)
            {
                parameters.Append($"{parameter.Key}={parameter.Value}&");
            }

            string requestUri = Constants.AzureManagementApi + $"subscriptions/{this.Subscription}/resourceGroups/{this.ResourceGroup}/{relativeUrl}{parameters.ToString()}api-version={apiVersion}";

            return await this.ExecuteGenericRequestWithHeaderAsync(method, requestUri, body);
        }

        public async Task<HttpResponseMessage> ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod method, string relativeUrl, string apiVersion, string body)
        {

            string requestUri = Constants.AzureManagementApi + $"subscriptions/{this.Subscription}/resourceGroups/{this.ResourceGroup}/{relativeUrl}?api-version={apiVersion}";

            return await this.ExecuteGenericRequestWithHeaderAsync(method, requestUri, body);
        }

        public async Task<HttpResponseMessage> ExecuteWithSubscriptionAsync(HttpMethod method, string relativeUrl, string apiVersion, string body)
        {
            string requestUri = Constants.AzureManagementApi + $"subscriptions/{this.Subscription}/{relativeUrl}?api-version={apiVersion}";

            return await this.ExecuteGenericRequestWithHeaderAsync(method, requestUri, body);
        }

        public async Task<HttpResponseMessage> ExecuteGenericRequestNoHeaderAsync(HttpMethod method, string url, string body)
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = url;
                HttpRequestMessage message = new HttpRequestMessage(method, requestUri);

                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    message.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                return await client.SendAsync(message);
            }
        }

        public async Task<HttpResponseMessage> ExecuteGenericRequestNoTokenAsync(HttpMethod method, string url, string body = "")
        {
            using (HttpClient client = new HttpClient())
            {
                string requestUri = url;

                HttpRequestMessage message = new HttpRequestMessage(method, requestUri);

                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    message.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                if (this.Headers != null)
                {
                    foreach (KeyValuePair<string, string> header in this.Headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/javascript"));

                SecurityHelper.SetTls12();
                return await client.SendAsync(message);
            }
        }

        public async Task<string> GetJson(HttpMethod method, string url, string body = "")
        {
            HttpResponseMessage response = await this.ExecuteGenericRequestNoTokenAsync(method, url, body);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }

        public async Task<HttpResponseDetails> GetJsonDetails(HttpMethod method, string url, string body = "")
        {
            HttpResponseMessage response = await this.ExecuteGenericRequestNoTokenAsync(method, url, body);
            return new HttpResponseDetails(response.StatusCode, await response.Content.ReadAsStringAsync());
        }

        public async Task<bool> IsSuccess(HttpMethod method, string url, string body = "")
        {
            HttpResponseMessage response = await this.ExecuteGenericRequestWithHeaderAsync(method, url, body);
            return response.IsSuccessStatusCode;
        }

        public async Task<string> Request(HttpMethod method, string url, string body = "")
        {
            HttpResponseMessage response = await this.ExecuteGenericRequestWithHeaderAsync(method, url, body);
            return response.IsSuccessStatusCode ? await response.Content.ReadAsStringAsync() : null;
        }

        public async Task<T> Request<T>(HttpMethod method, string url, string body = "")
        {
            T result = default(T);

            HttpResponseMessage response = await this.ExecuteGenericRequestWithHeaderAsync(method, url, body);

            if (response.IsSuccessStatusCode)
            {
                result = JsonUtility.Deserialize<T>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        public async Task<string> Request(string url, byte[] file, string name)
        {
            HttpResponseMessage response = null;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this.Token);

                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    content.Headers.ContentType.Parameters.Clear();
                    string boundary = GetFormBoundary();
                    content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("boundary", boundary));

                    HttpContent fileContent = new StreamContent(new MemoryStream(file));
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    fileContent.Headers.ContentDisposition.Name = "\"file0\"";
                    fileContent.Headers.ContentDisposition.FileName = $"\"{name}\"";
                    fileContent.Headers.ContentLength = null;
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-zip-compressed");

                    content.Add(fileContent);

                    response = await client.PostAsync(url, content);
                }
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<T> RequestAzure<T>(HttpMethod method, string url, string apiVersion, string body = "")
        {
            T result = default(T);

            HttpResponseMessage response = await this.ExecuteWithSubscriptionAndResourceGroupAsync(method, url, apiVersion, body);

            if (response.IsSuccessStatusCode)
            {
                result = JsonUtility.Deserialize<T>(await response.Content.ReadAsStringAsync());
            }

            return result;
        }

        public async Task<T> RequestValue<T>(HttpMethod method, string url, string body = "")
        {
            T value = default(T);

            HttpResponseMessage response = await this.ExecuteGenericRequestWithHeaderAsync(method, url, body);

            if (response.IsSuccessStatusCode)
            {
                value = JsonUtility.DeserializeContent<T>(await response.Content.ReadAsStringAsync());
            }

            return value;
        }

        public async Task<string> Test(HttpMethod method, string url, string body = "")
        {
            string message = null;

            HttpResponseMessage result = await this.ExecuteGenericRequestWithHeaderAsync(method, url, body);

            if (!result.IsSuccessStatusCode)
            {
                HttpErrorResponseWrapper response = JsonUtility.Deserialize<HttpErrorResponseWrapper>(await result.Content.ReadAsStringAsync());

                if (response != null && response.Error != null)
                {
                    message = response.Error.Message;
                }
                else
                {
                    message = result.StatusCode.ToString();
                }
            }

            return message;
        }

        private string GetFormBoundary()
        {
            return string.Format("---------------------------{0:N}", RandomGenerator.GetRandomHexadecimal(FORM_BOUNDARY_SIZE));
        }
    }
}