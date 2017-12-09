using System.Data;
using System.Data.SqlClient;

namespace RedditCore.DataModel.Repositories
{
    internal class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IDbConnection CreateDbConnection()
        {
            return new SqlConnection(this.configuration.DbConnectionString);
        }
    }
}
