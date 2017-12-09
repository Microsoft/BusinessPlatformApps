namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsInputSerialization
    {
        public StreamAnalyticsInputSerializationProperties Properties;
        public string Type;

        public StreamAnalyticsInputSerialization(string serialization)
        {
            Properties = new StreamAnalyticsInputSerializationProperties();
            Type = serialization;
        }
    }
}