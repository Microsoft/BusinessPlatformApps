using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsOutputSqlPropertiesWrapper
    {
        public StreamAnalyticsOutputSqlProperties Properties;

        public StreamAnalyticsOutputSqlPropertiesWrapper(BpstSql sql, string table)
        {
            Properties = new StreamAnalyticsOutputSqlProperties(sql, table);
        }
    }
}