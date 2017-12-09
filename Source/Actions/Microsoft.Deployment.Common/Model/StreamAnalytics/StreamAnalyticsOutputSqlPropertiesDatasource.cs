using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsOutputSqlPropertiesDatasource
    {
        public StreamAnalyticsOutputSqlPropertiesDatasourceProperties Properties;
        public string Type = "Microsoft.Sql/Server/Database";

        public StreamAnalyticsOutputSqlPropertiesDatasource(BpstSql sql, string table)
        {
            Properties = new StreamAnalyticsOutputSqlPropertiesDatasourceProperties(sql, table);
        }
    }
}