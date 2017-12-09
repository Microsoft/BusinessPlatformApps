using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Ninject;
using RedditCore;
using RedditAzureFunctions.Logging;
using RedditCore.AzureML;

namespace RedditAzureFunctions
{
    public static class ScheduledAzureML
    {
        private const string FunctionName = nameof(ScheduledAzureML);
        private const string ScheduledAzureMLFrequencyName = "%ScheduledAzureMLFrequency%";

        /// <summary>
        /// Perform application initialization on startup
        /// </summary>
        static ScheduledAzureML()
        {
            new Bootstrap().Init();
        }

        [FunctionName(FunctionName)]
        [Singleton(Mode = SingletonMode.Function)]
        public static async Task Run(
            [TimerTrigger(ScheduledAzureMLFrequencyName, RunOnStartup = WebServiceRunConstants.RunAmlOnStartup)] TimerInfo timer, // Every half hour
            [Queue(QueueConstants.AzureMLJobQueueName, Connection = QueueConstants.QueueConnectionStringName)] ICollector<string> queueCollector,
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
                var result = await processor.RunAzureMLProcessing();

                // If result is null then there is not any data to run through AzureML and no AML job was started.
                if (result != null)
                {
                    queueCollector.Add(result.JobId);
                    log.Verbose($"AzureML Web Service called; JobId=[{result.JobId}]");
                }
                else
                {
                    log.Verbose("No data to run through AzureML; no AML job started.");
                }
            }

            logger.Info($"{FunctionName} completed at {DateTime.Now}");
        }
    }
}
