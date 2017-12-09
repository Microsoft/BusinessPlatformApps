namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsQueryPropertiesWrapper
    {
        public StreamAnalyticsQueryProperties Properties;

        public StreamAnalyticsQueryPropertiesWrapper(string query)
        {
            Properties = new StreamAnalyticsQueryProperties(query);
        }
    }
}