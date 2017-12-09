using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Ninject;
using RedditCore;
using RedditAzureFunctions.Logging;
using RedditCore.SocialGist;
using RedditCore.DataModel;
using RedditCore.Telemetry;
using ExecutionContext = Microsoft.Azure.WebJobs.ExecutionContext;

namespace RedditAzureFunctions
{
    public static class ExecuteRedditSearch
    {
        private const string FunctionName = nameof(ExecuteRedditSearch); 

        /// <summary>
        /// Perform application initialization on startup
        /// </summary>
        static ExecuteRedditSearch()
        {
            new Bootstrap().Init();
        }

        [FunctionName(FunctionName)]
        public static void Run(
            [QueueTrigger(QueueConstants.RedditSearchQueueName, Connection = QueueConstants.QueueConnectionStringName)] string processMessage,
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

                var socialGist = kernel.Get<ISocialGist>();
                var telemetry = kernel.Get<ITelemetryClient>();
                socialGist.ResultLimitPerPage = webConfiguration.ResultLimitPerPage;
                socialGist.MaximumResultsPerSearch = webConfiguration.MaximumResultsPerSearch;

                SortedSet<SocialGistPostId> threadMatches = null;
                using (var sgQueryTelemetry =
                    telemetry.StartTrackDependency("Execute Search", null, "SocialGistPostSearch"))
                {
                    threadMatches = socialGist.MatchesForQuery(
                        webConfiguration.QueryTerms,
                        null
                    ).Result;

                    sgQueryTelemetry.IsSuccess = true;
                }

                logger.Info(
                    $"Returned [{threadMatches.Count}] posts from search terms [{webConfiguration.QueryTerms}]");

                using (var queueCollectorTelemetry =
                    telemetry.StartTrackDependency("Enqueue Results", null, "SocialGistPostSearch"))
                {

                    var timeDelay = webConfiguration.SearchToThreadTimeDelay;
                    var queue = CreateQueue();

                    queue.CreateIfNotExists();

                    QueueRequestOptions queueRequestOptions = new QueueRequestOptions()
                    {
                        MaximumExecutionTime = TimeSpan.FromMinutes(1)
                    };

                    var q = new HashSet<int>();

                    // Write to the queue.  By default this will use will utilize however many threads the underlying scheduler provides.
                    // See https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.paralleloptions.maxdegreeofparallelism?view=netframework-4.7.1#System_Threading_Tasks_ParallelOptions_MaxDegreeOfParallelism
                    Parallel.ForEach<SocialGistPostId, CloudQueue>(
                        threadMatches,
                        CreateQueue,
                        (item, loopState, innerQueue) =>
                        {
                            q.Add(Thread.CurrentThread.ManagedThreadId);
                            var messageContent = JsonConvert.SerializeObject(item);
                            var message = new CloudQueueMessage(messageContent);
                            innerQueue.AddMessage(message, options: queueRequestOptions,
                                initialVisibilityDelay: timeDelay);
                            return innerQueue;
                        },
                        (finalResult) => { }
                    );

                    queueCollectorTelemetry.Properties.Add("Total Number of Threads Used", q.Count.ToString());
                    queueCollectorTelemetry.IsSuccess = true;
                }

                var metric = new MetricTelemetry()
                {
                    Name = "Unique posts returned by search",
                    Sum = threadMatches.Count,
                    Timestamp = DateTime.Now,
                    Properties =
                    {
                        {"QueryTerms", webConfiguration.QueryTerms },
                    }
                };
                telemetry.TrackMetric(metric);
            }

            logger.Info($"{FunctionName} completed at {DateTime.Now}");
        }

        private static CloudQueue CreateQueue()
        {
            var connectionString = ConfigurationManager.AppSettings[QueueConstants.QueueConnectionStringName];
            var queueName = ConfigurationManager.AppSettings[QueueConstants.RedditPostQueueNameProperty];
            var storageAccount = CloudStorageAccount.Parse(connectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            return queue;
        }
    }
}
