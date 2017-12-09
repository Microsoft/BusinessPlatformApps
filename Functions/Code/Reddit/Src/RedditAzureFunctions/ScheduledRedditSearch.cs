using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace RedditAzureFunctions
{
    public static class ScheduledRedditSearch
    {
        private const string FunctionName = nameof(ScheduledRedditSearch);
        private const string ScheduledRedditQueryFrequencyName = "%ScheduledRedditQueryFrequency%";

        /// <summary>
        /// Perform application initialization on startup
        /// </summary>
        static ScheduledRedditSearch()
        {
            new Bootstrap().Init();
        }

        [FunctionName(FunctionName)]
        public static void Run(
            [TimerTrigger(ScheduledRedditQueryFrequencyName, RunOnStartup = WebServiceRunConstants.RunRedditOnStartup)] TimerInfo timer, 
            [Queue(QueueConstants.RedditSearchQueueName, Connection = QueueConstants.QueueConnectionStringName)] ICollector<string> queueCollector,
            ExecutionContext executionContext,
            TraceWriter logger
        )
        {
            logger.Info($"{FunctionName} initiated by timer at {DateTime.Now}");
            queueCollector.Add("begin"); 
            logger.Info($"{FunctionName} completed at {DateTime.Now}");
        }
    }
}
