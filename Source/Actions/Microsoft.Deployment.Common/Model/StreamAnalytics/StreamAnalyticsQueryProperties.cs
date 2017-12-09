namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsQueryProperties
    {
        public string Query;
        public int StreamingUnits = 1;

        public StreamAnalyticsQueryProperties(string query)
        {
            Query = query;
        }
    }
}