using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class UserDefinedEntityRepository : RepositoryBase<UserDefinedEntity>
    {
        [Inject]
        public UserDefinedEntityRepository(EntityConnection connection, ILog log)
            : base(connection, log, "userDefinedEntity")
        {
        }

        protected override DbSet<UserDefinedEntity> GetDbSet(RedditEntities db)
        {
            return db.UserDefinedEntities;
        }
    }
}