using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Ninject;
using RedditAzureFunctions.Logging;
using RedditCore;
using RedditCore.DataModel;
using RedditCore.SocialGist;
using System;

namespace RedditAzureFunctions
{
    public static class ProcessPost
    {
        private const string FunctionName = nameof(ProcessPost);

        /// <summary>
        /// Perform application initialization on startup
        /// </summary>
        static ProcessPost()
        {
            new Bootstrap().Init();
        }

        [FunctionName(FunctionName)]
        public static async Task ProcessRedditPost(
            [QueueTrigger(QueueConstants.RedditPostQueueName, Connection = QueueConstants.QueueConnectionStringName)] SocialGistPostId socialGistPost, 
            TraceWriter log,
            ExecutionContext executionContext
        )
        {
            log.Info($"{FunctionName} Execution begun at {DateTime.Now}");

            var config = new WebConfiguration(executionContext);
            var logger = new FunctionLog(log, executionContext.InvocationId);
            var objectLogger = (config.UseObjectLogger) ? new BlobObjectLogger(config, logger) : null;

            using (var kernel = new KernelFactory().GetKernel(logger, config, objectLogger))
            {
                var processor = kernel.Get<IThreadProcessor>();
                var socialGist = kernel.Get<ISocialGist>();
                socialGist.ResultLimitPerPage = config.ResultLimitPerPage;
                socialGist.MaximumResultsPerSearch = config.MaximumResultsPerSearch;

                await processor.Process(socialGistPost);
            }

            log.Info($"{FunctionName} completed at {DateTime.Now}");
        }
    }
}
