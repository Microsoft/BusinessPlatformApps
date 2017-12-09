using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsOutputSqlProperties
    {
        public StreamAnalyticsOutputSqlPropertiesDatasource Datasource;

        public StreamAnalyticsOutputSqlProperties(BpstSql sql, string table)
        {
            Datasource = new StreamAnalyticsOutputSqlPropertiesDatasource(sql, table);
        }
    }
}