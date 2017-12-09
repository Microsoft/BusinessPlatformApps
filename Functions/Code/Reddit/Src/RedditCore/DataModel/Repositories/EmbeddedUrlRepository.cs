using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class EmbeddedUrlRepository : RepositoryBase<EmbeddedUrl>
    {
        [Inject]
        public EmbeddedUrlRepository(EntityConnection connection, ILog log)
            : base(connection, log, "embeddedUrl")
        {
        }

        protected override DbSet<EmbeddedUrl> GetDbSet(RedditEntities db)
        {
            return db.EmbeddedUrls;
        }
    }
}
