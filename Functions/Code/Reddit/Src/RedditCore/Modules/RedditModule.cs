using Ninject.Modules;
using RedditCore.AzureML;
using RedditCore.SocialGist;

namespace RedditCore.Modules
{
    internal class RedditModule : NinjectModule
    {
        public override void Load()
        {
            // AzureML
            Bind<IExperimentCompletionWaiter>().To<ExperimentCompletionWaiter>();
            Bind<IAzureMLExperimentRunner>().To<AzureMLExperimentRunner>();
            Bind<IScheduledAzureMLProcessor>().To<ScheduledAzureMLProcessor>();

            // Paginators
            Bind<IApiPaginator>().To<ApiPaginator>();
        }
    }
}
