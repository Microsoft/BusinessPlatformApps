using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsCategory
    {
        [JsonProperty("value")]
        public string Value;

        [JsonProperty("localizedValue")]
        public string LocalizedValue;
    }
}
