using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class UserDefinedEntityDefinitionRepository : RepositoryBase<UserDefinedEntityDefinition>
    {
        [Inject]
        public UserDefinedEntityDefinitionRepository(EntityConnection connection, ILog log)
            : base(connection, log, "userDefinedEntityDefinition")
        {
        }

        protected override DbSet<UserDefinedEntityDefinition> GetDbSet(RedditEntities db)
        {
            return db.UserDefinedEntityDefinitions;
        }
    }
}
