namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsInputProperties
    {
        public StreamAnalyticsInputDatasource Datasource;
        public StreamAnalyticsInputSerialization Serialization;
        public string Type = "stream";

        public StreamAnalyticsInputProperties(string nameEventHub, string nameNamespace, string key, string serialization)
        {
            Datasource = new StreamAnalyticsInputDatasource(nameEventHub, nameNamespace, key);
            Serialization = new StreamAnalyticsInputSerialization(serialization);
        }
    }
}