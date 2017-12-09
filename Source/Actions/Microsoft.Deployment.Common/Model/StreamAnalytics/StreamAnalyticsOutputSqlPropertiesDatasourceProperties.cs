using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsOutputSqlPropertiesDatasourceProperties
    {
        public string Database;
        public string Password;
        public string Server;
        public string Table;
        public string User;

        public StreamAnalyticsOutputSqlPropertiesDatasourceProperties(BpstSql sql, string table)
        {
            Database = sql.ConnectionDatabase;
            Password = sql.UserPassword;
            Server = sql.ConnectionServer;
            Table = table;
            User = sql.UserName;
        }
    }
}