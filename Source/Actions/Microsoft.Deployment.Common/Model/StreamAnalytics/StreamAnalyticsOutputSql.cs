using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsOutputSql
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("table")]
        public string Table;
    }
}