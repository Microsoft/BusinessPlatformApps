using RedditCore.Logging;
using System;
using System.Data;
using System.Threading.Tasks;
using RedditCore.DataModel.Repositories;
using RedditCore.Properties;
using RedditCore.Telemetry;

namespace RedditCore.AzureML
{
    internal class ScheduledAzureMLProcessor : IScheduledAzureMLProcessor
    {
        private readonly IAzureMLExperimentRunner experimentRunner;
        private readonly IExperimentCompletionWaiter completionWaiter;
        private readonly IDbConnectionFactory connectionFactory;
        private readonly ITelemetryClient telemetryClient;
        private readonly ILog log;

        public ScheduledAzureMLProcessor(
            IAzureMLExperimentRunner experimentRunner,
            IExperimentCompletionWaiter waiter,
            IDbConnectionFactory connectionFactory,
            ITelemetryClient telemetryClient,
            ILog log)
        {
            this.experimentRunner = experimentRunner;
            this.connectionFactory = connectionFactory;
            this.log = log;
            this.completionWaiter = waiter;
            this.telemetryClient = telemetryClient;
        }

        public async Task<AzureMLResult> RunAzureMLProcessing()
        {
            if (DataReadyForAzureMachineLearning())
            {
                this.telemetryClient.TrackEvent(TelemetryNames.ScheduledAzureMLProcessor_DataFound);

                var azureMLResult = await experimentRunner.RunExperimentWithData(null, false);

                if (!azureMLResult.Success.GetValueOrDefault(true))
                    throw new ScheduledAzureMLProcessorException(
                        $"Error in AzureML pipeline: {azureMLResult.Error}  Details: {azureMLResult.Details}");

                return azureMLResult;
            }
            else
            {
                this.telemetryClient.TrackEvent(TelemetryNames.ScheduledAzureMLProcessor_NoDataFound);
                return null;
            }
        }

        private bool DataReadyForAzureMachineLearning()
        {
            using (var connection = connectionFactory.CreateDbConnection())
            {
                connection.Open();

                // SELECT a null row for each row in AzureMachineLearningInputView.
                // If any rows are found then return 1.  If no rows are found then return 0.
                // This was done to try and avoid SELECT COUNT(*) FROM AzureMachineLearningInputView and then checking the count.
                const string query = @"
                    SELECT ( 
                        CASE WHEN EXISTS(SELECT TOP 5 NULL FROM [reddit].[AzureMachineLearningInputView])
	                    THEN 1
	                    ELSE 0
	                    END
	                ) AS isNotEmpty";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;

                    var result = command.ExecuteScalar();

                    // If the command returns 1 then we have data to process!
                    return result?.ToString() == "1";
                }
            }
        }

        public async Task<AzureMLResult> CheckAzureMLAndPostProcess(string jobId)
        {
            var azureMlResult = await completionWaiter.WaitForJobCompletion(jobId, TimeSpan.Zero, false);

            if (!azureMlResult.LastJobStatus.IsTerminalState()) return azureMlResult;
            log.Info("AzureML completed.  Ready for staging migration");

            using (var connection = connectionFactory.CreateDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    // Set the timeout to five minutes.  This may be a long task
                    command.CommandTimeout = 300;

                    command.CommandText = "reddit.PostAzureML";
                    command.CommandType = CommandType.StoredProcedure;

                    log.Info("Pushing to live tables from staging");
                    int result = command.ExecuteNonQuery();
                    log.Info($"Pushed to live tables from staging.  Stored Procedure result: {result}");
                }
            }

            return azureMlResult;
        }
    }
}
