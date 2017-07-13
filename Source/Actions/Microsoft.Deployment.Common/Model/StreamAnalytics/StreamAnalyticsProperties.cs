using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsProperties
    {
        [JsonProperty("statusCode")]
        public string StatusCode;

        [JsonProperty("eventSource")]
        public string EventSource;

        [JsonProperty("Region")]
        public string ImpactedRegions;

        [JsonProperty("Transcript of Communication")]
        public string Impact;
    }
}
