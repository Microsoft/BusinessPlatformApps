using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.AzureML
{
    internal class AzureMLExperimentRunner : IAzureMLExperimentRunner
    {
        private readonly IExperimentCompletionWaiter completionWaiter;
        private readonly IConfiguration configuration;
        private readonly ILog log;
        private readonly IObjectLogger objectLogger;

        [Inject]
        public AzureMLExperimentRunner(
            ILog log, 
            IConfiguration configuration,
            IExperimentCompletionWaiter completionWaiter,
            IObjectLogger objectLogger)
        {
            this.log = log;
            this.configuration = configuration;
            this.completionWaiter = completionWaiter;
            this.objectLogger = objectLogger;
        }

        public async Task<AzureMLResult> RunExperimentWithData(string data, bool waitForCompletion)
        {
            var globalParameters = new Dictionary<string, string>();

            var builder = new SqlConnectionStringBuilder(configuration.DbConnectionString);

            if (data != null)
            {
                globalParameters.Add("Data", data);
            }
            globalParameters.Add("Database server name", builder.DataSource);
            globalParameters.Add("Database name", builder.InitialCatalog);
            globalParameters.Add("Server user account name", builder.UserID);
            globalParameters.Add("Server user account password", builder.Password);

            using (var client = new HttpClient())
            {
                var baseUrl = configuration.AzureMLBaseUrl.TrimEndString("?api-version=2.0");

                var request = new BatchExecutionRequest
                {
                    GlobalParameters = globalParameters
                };

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", configuration.AzureMLApiKey);

                log.Info("Submitting the job...", "AzureMLExperimentRunner");

                // submit the job
                var serialized = JsonConvert.SerializeObject(request);

                HttpContent httpContent = new ByteArrayContent(Encoding.ASCII.GetBytes(serialized));
                httpContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                var response = await client.PostAsync(
                    $"{baseUrl}?api-version=2.0", 
                    httpContent
                );

                if (!response.IsSuccessStatusCode)
                    return new AzureMLResult {Success = false, Error = "Unable to submit job", Details = response.ToString()};

                var jobIdBytes = await response.Content.ReadAsByteArrayAsync();

                // Job ID is returned with quotes.  Remove the quotes.
                var jobId = Encoding.ASCII.GetString(jobIdBytes).Trim('"');
                this.objectLogger.Log(data, "AzureMLExperimentRunner", $"Submitted jobId: {jobId}");
                log.Info($"Submitted job ID: {jobId}", "AzureMLExperimentRunner");

                // start the job
                log.Info("Starting the job...");
                response = await client.PostAsync(
                    $"{baseUrl}/{jobId}/start?api-version=2.0",
                    null
                );
                if (!response.IsSuccessStatusCode)
                    return new AzureMLResult {Success = false, Error = $"Unable to start job {jobId}"};

                if (waitForCompletion)
                    return await completionWaiter.WaitForJobCompletion(jobId);
                return new AzureMLResult
                {
                    // Set success to null because we did not wait for completion.  We do not know if the job was successful or not.
                    Success = null,
                    LastJobStatus = BatchScoreStatusCode.Unknown,
                    JobId = jobId
                };
            }
        }
    }
}
