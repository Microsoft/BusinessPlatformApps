using Ninject.Modules;

namespace RedditCore.SocialGist.DocumentProcessors
{
    internal class DocumentProcessorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDocumentProcessor>().To<UserDefinedEntityProcessor>();
            Bind<IDocumentProcessor>().To<EmbeddedLinksProcessor>();
        }
    }
}
