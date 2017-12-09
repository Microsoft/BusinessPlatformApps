using System;
using System.Configuration;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Ninject;
using RedditCore;
using RedditAzureFunctions.Logging;
using RedditCore.AzureML;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace RedditAzureFunctions
{
    public static class AzureMLJobChecker
    {
        /// <summary>
        /// Perform application initialization on startup
        /// </summary>
        static AzureMLJobChecker()
        {
            new Bootstrap().Init();
        }

        private const string FunctionName = nameof(AzureMLJobChecker);

        [FunctionName(FunctionName)]
        [Singleton(Mode = SingletonMode.Function)]
        public static async Task Run(
            [QueueTrigger(QueueConstants.AzureMLJobQueueName, Connection = QueueConstants.QueueConnectionStringName)] string jobId, 
            ExecutionContext executionContext,
            TraceWriter logger
        )
        {
            logger.Info($"{FunctionName} Execution begun at {DateTime.Now}");
            IConfiguration webConfiguration = new WebConfiguration(executionContext);
            var log = new FunctionLog(logger, executionContext.InvocationId);

            var objectLogger = (webConfiguration.UseObjectLogger) ? new BlobObjectLogger(webConfiguration, log) : null;

            using (var kernel = new KernelFactory().GetKernel(
                log,
                webConfiguration,
                objectLogger
            ))
            {

                var processor = kernel.Get<IScheduledAzureMLProcessor>();
                var result = await processor.CheckAzureMLAndPostProcess(jobId);

                if (!result.LastJobStatus.IsTerminalState())
                {
                    var queue = CreateQueue();

                    // Job is not finished.  Put it back on the queue to try again.
                    var message = new CloudQueueMessage(jobId);
                    queue.AddMessage(message, null, webConfiguration.AzureMlRetryTimeDelay);
                }
            }

            logger.Info($"{FunctionName} completed at {DateTime.Now}");
        }

        private static CloudQueue CreateQueue()
        {
            var connectionString = ConfigurationManager.AppSettings[QueueConstants.QueueConnectionStringName];
            var queueName = ConfigurationManager.AppSettings[QueueConstants.AzureMLJobQueueNameProperty];
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            return queue;
        }
    }
}
