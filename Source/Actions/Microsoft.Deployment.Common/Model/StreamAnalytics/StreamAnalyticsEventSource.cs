using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsEventSource
    {
        [JsonProperty("value")]
        public string Value;

        [JsonProperty("localizedValue")]
        public string LocalizedValue;
    }
}
