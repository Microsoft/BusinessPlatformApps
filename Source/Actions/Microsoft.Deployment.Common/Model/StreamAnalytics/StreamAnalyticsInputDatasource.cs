namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsInputDatasource
    {
        public StreamAnalyticsInputDatasourceProperties Properties;
        public string Type = "Microsoft.ServiceBus/EventHub";

        public StreamAnalyticsInputDatasource(string nameEventHub, string nameNamespace, string key)
        {
            Properties = new StreamAnalyticsInputDatasourceProperties(nameEventHub, nameNamespace, key);
        }
    }
}