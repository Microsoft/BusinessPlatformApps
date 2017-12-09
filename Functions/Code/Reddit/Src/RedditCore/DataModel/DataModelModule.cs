using System.Data.Entity.Core.EntityClient;
using Ninject.Modules;
using RedditCore.DataModel.Repositories;

namespace RedditCore.DataModel
{
    public class DataModelModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IRepository<Post>>().To<PostRepository>();
            Bind<IRepository<Comment>>().To<CommentRepository>();
            Bind<IRepository<UserDefinedEntityDefinition>>().To<UserDefinedEntityDefinitionRepository>();
            Bind<IRepository<UserDefinedEntity>>().To<UserDefinedEntityRepository>();
            Bind<IRepository<EmbeddedUrl>>().To<EmbeddedUrlRepository>();
            Bind<IRepository<PostCommentCount>>().To<PostCommentCountRepository>();
            Bind<IDocumentRemover>().To<DocumentRemover>();

            // Singleton scope is needed to make sure this is disposed when the kernel is disposed
            Bind<EntityConnection>().ToProvider<EntityConnectionProvider>().InSingletonScope();

            Bind<IDbConnectionFactory>().To<DbConnectionFactory>().InSingletonScope();
        }
    }
}
