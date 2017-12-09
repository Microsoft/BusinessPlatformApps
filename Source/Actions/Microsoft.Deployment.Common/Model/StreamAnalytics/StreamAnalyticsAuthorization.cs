using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsAuthorization
    {
        [JsonProperty("action")]
        public string Action;

        [JsonProperty("scope")]
        public string Scope;
    }
}
