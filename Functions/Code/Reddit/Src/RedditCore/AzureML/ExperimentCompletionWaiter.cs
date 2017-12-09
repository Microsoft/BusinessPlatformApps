using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Ninject;
using RedditCore.Logging;
using RedditCore.Http;
using System.Net.Http.Headers;
using System;

namespace RedditCore.AzureML
{
    internal class ExperimentCompletionWaiter : IExperimentCompletionWaiter
    {
        private readonly IConfiguration configuration;
        private readonly ILog log;
        private readonly IHttpClient httpClient;

        [Inject]
        public ExperimentCompletionWaiter(
            ILog log,
            IConfiguration configuration,
            IHttpClient client)
        {
            this.configuration = configuration;
            this.log = log;
            this.httpClient = client;
        }

        public async Task<AzureMLResult> WaitForJobCompletion(string jobId)
        {
            return await WaitForJobCompletion(jobId, TimeSpan.FromMinutes(10), true);
        }

        public async Task<AzureMLResult> WaitForJobCompletion(string jobId, TimeSpan timeout, bool killJobAfterTimeout)
        {
            var jobLocation = new Uri(configuration.AzureMLBaseUrl.TrimEndString("?api-version=2.0") + "/" + jobId + "?api-version=2.0");
            var done = false;

            bool? success = false;
            var message = "";
            var details = "";

            // Set a default.  "Unknown" is not a valid response and AzureML will never return it.
            BatchScoreStatusCode lastStatus = BatchScoreStatusCode.Unknown;

            var watch = Stopwatch.StartNew();

            var authenticationHeaderValue = new AuthenticationHeaderValue("Bearer", configuration.AzureMLApiKey);

            while (!done)
            {
                log.Info("Checking the job status...");
                var response = await httpClient.GetJsonAsync<BatchScoreStatus>(
                    jobLocation,
                    authenticationHeaderValue: authenticationHeaderValue);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                    return new AzureMLResult { Success = false, Error = $"Error in Job ID {jobId}" };

                if (watch.ElapsedMilliseconds > timeout.TotalMilliseconds)
                {
                    if (killJobAfterTimeout)
                    {
                        done = true;
                        log.Info(string.Format("Timed out. Deleting job {0} ...", jobId));
                        message = "ExperimentCompletionWaiter killed the job due to timeout";
                        await httpClient.DeleteAsync(jobLocation, authenticationHeaderValue: authenticationHeaderValue);
                    }
                    else
                    {
                        done = true;
                        message = "ExperimentCompletionWaiter timed out without canceling the job";
                        log.Info($"Timed out. Leaving job {jobId} running ...");
                    }
                }

                var status = response.Object;
                lastStatus = status.StatusCode; 
                log.Verbose($"Job {jobId} status {status.StatusCode}");

                switch (status.StatusCode)
                {
                    case BatchScoreStatusCode.NotStarted:
                        log.Info(string.Format("Job {0} not yet started...", jobId));
                        break;
                    case BatchScoreStatusCode.Running:
                        log.Info(string.Format("Job {0} running...", jobId));
                        break;
                    case BatchScoreStatusCode.Failed:
                        log.Info(string.Format("Job {0} failed!", jobId));
                        log.Info(string.Format("Error details: {0}", status.Details));
                        done = true;
                        message = $"Job {jobId} failed";
                        details = status.Details;
                        break;
                    case BatchScoreStatusCode.Cancelled:
                        log.Info(string.Format("Job {0} cancelled!", jobId));
                        done = true;
                        message = $"Cancelled job {jobId}";
                        break;
                    case BatchScoreStatusCode.Finished:
                        done = true;
                        log.Info(string.Format("Job {0} finished!", jobId));
                        log.Info("Response: ");
                        log.Info(await response.ResponseMessage.Content.ReadAsStringAsync());
                        success = true;
                        break;
                }

                if (!done)
                    Thread.Sleep(1000); // Wait one second
            }

            return new AzureMLResult
            {
                Success = success,
                Message = message,
                Details = details,
                LastJobStatus = lastStatus,
                JobId = jobId
            };
        }
    }
}
