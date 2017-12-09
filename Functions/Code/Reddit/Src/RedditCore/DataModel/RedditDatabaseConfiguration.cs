using System.Data.Entity;
using System.Data.Entity.SqlServer;

namespace RedditCore.DataModel
{
    public class RedditDatabaseConfiguration : DbConfiguration
    {
        public RedditDatabaseConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());
        }
    }
}
