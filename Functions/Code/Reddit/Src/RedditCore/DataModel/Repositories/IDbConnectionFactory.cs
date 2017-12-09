using System.Data;

namespace RedditCore.DataModel.Repositories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateDbConnection();
    }
}
