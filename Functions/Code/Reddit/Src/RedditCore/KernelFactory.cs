using Ninject;
using RedditCore.DataModel;
using RedditCore.DocumentAggregators;
using RedditCore.DocumentFilters;
using RedditCore.Http;
using RedditCore.Logging;
using RedditCore.Modules;
using RedditCore.SocialGist;
using RedditCore.SocialGist.DocumentProcessors;
using RedditCore.Telemetry;

namespace RedditCore
{
    public class KernelFactory
    {
        /// <summary>
        /// Gets a new Kernel
        /// </summary>
        /// <param name="log"></param>
        /// <param name="configuration"></param>
        /// <param name="objectLogger">Logger that logs objects to a permanant store for debugging later.  If NULL then logger is a NoOp.</param>
        /// <returns></returns>
        public IKernel GetKernel(
            ILog log, 
            IConfiguration configuration, 
            IObjectLogger objectLogger = null)
        {
            var olog = (objectLogger != null) ? objectLogger : new NoOpObjectLogger();

            return new StandardKernel(
                new RedditModule(),
                new SocialGistModule(),
                new ConfigurationModule(configuration, log, olog),
                new HttpModule(),
                new DocumentProcessorsModule(),
                new TelemetryModule(),
                new DocumentFiltersModule(configuration),
                new DocumentAggregatorsModule(),
                new DataModelModule()
            );
        }
    }
}