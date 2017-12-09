using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using RedditCore.Logging;
using RedditCore.Properties;
using System.Collections.Generic;
using RedditCore.Telemetry;
using System.Threading;
using Microsoft.ApplicationInsights.DataContracts;

namespace RedditCore.Http
{
    internal class HttpClient : IHttpClient
    {
        private readonly ILog log;
        private readonly IObjectLogger objectLogger;
        private readonly ITelemetryClient telemetryClient;

        private const int RETRY_COUNT = 5;

        private readonly TimeSpan minWaitBeforeRetry = TimeSpan.FromSeconds(15);
        private readonly TimeSpan maxWaitBeforeRetry = TimeSpan.FromSeconds(45);

        [Inject]
        public HttpClient(
            ILog log,
            ITelemetryClient telemetryClient,
            IObjectLogger objectLogger)
        {
            this.log = log;
            this.objectLogger = objectLogger;
            this.telemetryClient = telemetryClient;
        }

        public async Task<IHttpJsonResponseMessage<T>> GetJsonAsync<T>(
            Uri requestUri,
            TimeSpan? requestTimeout = null,
            AuthenticationHeaderValue authenticationHeaderValue = null)
        {
            // SocialGist only accepts TLS 1.1 or higher.  AzureFunctions (on Azure) default to SSL3 or TLS
            // AzureFunctions runing locally use defaults that work with SocialGist but remote Azure Functions do not.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            log.Verbose($"Retrieving URL using protocol: {ServicePointManager.SecurityProtocol}");
            telemetryClient.TrackEvent(TelemetryNames.HTTP_Get, new Dictionary<string, string> { { "URL", requestUri.ToString() } });

            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            using (var client = new System.Net.Http.HttpClient(handler))
            {
                if (requestTimeout.HasValue)
                {
                    client.Timeout = requestTimeout.Value;
                }

                if (authenticationHeaderValue != null)
                {
                    client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                }

                string data = "";
                HttpResponseMessage response = null;
                int retryCount = 0;
                while (retryCount < RETRY_COUNT)
                {
                    try
                    {
                        telemetryClient.TrackEvent(TelemetryNames.HTTP_Try, new Dictionary<string, string> { { "URL", requestUri?.ToString() }, { "try number", retryCount.ToString() } });
                        log.Verbose($"Retrieving URL {requestUri} on attempt {retryCount}");

                        // Track telemetry for each host query
                        using (var tracker = telemetryClient.StartTrackDependency(requestUri.Host, requestUri.AbsolutePath, "HTTP"))
                        {
                            tracker.Properties.Add("Request URL", requestUri?.ToString());
                            tracker.Properties.Add("Try attempt", retryCount.ToString());

                            try
                            {
                                response = await client.GetAsync(requestUri);
                            }
                            catch(TaskCanceledException)
                            {
                                tracker.IsSuccess = false;
                                tracker.ResultCode = "Task Canceled (HTTP Timeout)";
                                throw;
                            }
                            catch(HttpRequestException e)
                            {
                                tracker.IsSuccess = false;
                                tracker.ResultCode = $"HTTPException: {e.Message}";
                                throw;
                            }

                            tracker.IsSuccess = response.IsSuccessStatusCode;
                            tracker.ResultCode = response.StatusCode.ToString();

                            // Get data after setting telemetry status codes incase this errors out.
                            data = await response.Content.ReadAsStringAsync();

                            if(!response.IsSuccessStatusCode)
                            {
                                // Do not store this data unless the request failed.  This data can get very large very fast.
                                tracker.Properties.Add("Server Response", data);
                            }
                        }

                        if (response.IsSuccessStatusCode)
                        {
                            this.objectLogger.Log(data, "HttpClient", requestUri.ToString());

                            // Successfully completed request
                            break;
                        }
                        else
                        {
                            telemetryClient.TrackEvent(TelemetryNames.HTTP_Error, new Dictionary<string, string> {
                                { "URL", requestUri?.ToString() },
                                { "StatusCode", response?.StatusCode.ToString() }
                            });
                            retryCount = HandleFailure(retryCount, requestUri, response);
                        }
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        telemetryClient.TrackEvent(TelemetryNames.HTTP_Error, new Dictionary<string, string> {
                            { "URL", requestUri?.ToString() },
                            { "StatusCode", response?.StatusCode.ToString() },
                            { "Exception", e.ToString() }
                        });
                        if (retryCount < RETRY_COUNT)
                        {
                            log.Error($"Error executing web request for URL {requestUri} on attempt {retryCount}.  Retrying.", e);
                        }
                        else
                        {
                            log.Error($"Error executing web request for URI {requestUri} on attempt {retryCount}.  NOT retrying", e);

                            // Rethrow the exception to fail the current job
                            throw;
                        }
                    }

                    // Do not wait after the final try.
                    if (retryCount < RETRY_COUNT)
                    {
                        // Request failed.  Wait a random time and try again.  Random() uses a time-dependent seed.
                        int wait = new Random().Next((int)minWaitBeforeRetry.TotalMilliseconds, (int)maxWaitBeforeRetry.TotalMilliseconds);
                        log.Verbose($"Waiting {wait} milliseconds before retrying");
                        Thread.Sleep(wait);
                    }
                }

                log.Verbose($"URL {requestUri} retrieved.");

                var metric = new MetricTelemetry();
                metric.Name = TelemetryNames.HTTP_RetryCount;
                metric.Sum = retryCount;
                metric.Timestamp = DateTime.Now;
                metric.Properties.Add("Domain", requestUri.Host);
                telemetryClient.TrackMetric(metric);

                try
                {
                    var obj = JsonConvert.DeserializeObject<T>(data);

                    log.Verbose($"Result deserialized from URL {requestUri}");

                    return new HttpJsonResponseMessage<T>
                    {
                        Object = obj,
                        ResponseMessage = response
                    };
                }
                catch (Exception e)
                {
                    telemetryClient.TrackEvent(TelemetryNames.HTTP_JSON_Error, new Dictionary<string, string>
                    {
                        {"URL", requestUri?.ToString()},
                        {"StatusCode", response?.StatusCode.ToString()},
                        {"Exception", e.ToString()},
                        {"Response", data}
                    });

                    // Log and rethrow the exception
                    log.Error($"Error deserializing result from URL {requestUri}", e);

                    // Rethrow the exception to fail the current job
                    throw;
                }
            }
        }

        private int HandleFailure(int retryCount, Uri requestUri, HttpResponseMessage response)
        {
            var result = retryCount + 1;
            if (result < RETRY_COUNT)
            {
                log.Warning($"Error executing web request for URL {requestUri} on attempt {result}.  Response code: {response.StatusCode}.  Retrying.");
            }
            else
            {
                log.Warning($"Error executing web request for URI {requestUri} on attempt {result}.  Response code: {response.StatusCode}.  NOT retrying");
            }

            return result;
        }

        public async Task<HttpResponseMessage> DeleteAsync(Uri requestUri, TimeSpan? requestTimeout = null, AuthenticationHeaderValue authenticationHeaderValue = null)
        {
            // SocialGist only accepts TLS 1.1 or higher.  AzureFunctions (on Azure) default to SSL3 or TLS
            // AzureFunctions runing locally use defaults that work with SocialGist but remote Azure Functions do not.
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            log.Verbose($"Retrieving URL using protocol: {ServicePointManager.SecurityProtocol}");
            using (var client = new System.Net.Http.HttpClient())
            {
                log.Verbose($"Retrieving URL {requestUri}");

                if (requestTimeout.HasValue)
                {
                    client.Timeout = requestTimeout.Value;
                }

                if (authenticationHeaderValue != null)
                {
                    client.DefaultRequestHeaders.Authorization = authenticationHeaderValue;
                }

                HttpResponseMessage response = null;
                int retryCount = 0;
                while (retryCount < RETRY_COUNT)
                {
                    try
                    {
                        response = await client.GetAsync(requestUri);

                        if (response.IsSuccessStatusCode)
                        {
                            return response;
                        }
                        else
                        {
                            retryCount = HandleFailure(retryCount, requestUri, response);
                        }
                    }
                    catch (Exception e)
                    {
                        retryCount++;
                        if (retryCount < RETRY_COUNT)
                        {
                            log.Error($"Error executing web request for URL {requestUri} on attempt {retryCount}.  Retrying.", e);
                        }
                        else
                        {
                            log.Error($"Error executing web request for URI {requestUri} on attempyt {retryCount}.  NOT retrying", e);

                            // Rethrow the exception to fail the current job
                            throw;
                        }
                    }
                }

                return response;
            }
        }
    }
}
