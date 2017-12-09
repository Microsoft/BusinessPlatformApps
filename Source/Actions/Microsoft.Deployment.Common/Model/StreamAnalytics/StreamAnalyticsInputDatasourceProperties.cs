namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsInputDatasourceProperties
    {
        public string EventHubName;
        public string ServiceBusNamespace;
        public string SharedAccessPolicyKey;
        public string SharedAccessPolicyName = "RootManageSharedAccessKey";

        public StreamAnalyticsInputDatasourceProperties(string nameEventHub, string nameNamespace, string key)
        {
            EventHubName = nameEventHub;
            SharedAccessPolicyKey = key;
            ServiceBusNamespace = nameNamespace;
        }
    }
}