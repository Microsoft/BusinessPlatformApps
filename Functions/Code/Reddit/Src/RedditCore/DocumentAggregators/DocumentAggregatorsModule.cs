using Ninject.Modules;

namespace RedditCore.DocumentAggregators
{
    internal class DocumentAggregatorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentAggregator>().To<CommentCountAggregator>();
        }
    }
}
