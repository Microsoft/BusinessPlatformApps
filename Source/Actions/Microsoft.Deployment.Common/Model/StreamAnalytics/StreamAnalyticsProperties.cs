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

        [JsonProperty("Service")]
        public string ImpactedServices;

        [JsonProperty("Transcript of Communication")]
        public string Impact;

        [JsonProperty("IncidentType")]
        public string IncidentType;

        [JsonProperty("Title")]
        public string Title;
    }
}
