using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsHttpRequest
    {
        [JsonProperty("clientRequestId")]
        public string ClientRequestId;

        [JsonProperty("clientIpAddress")]
        public string ClientIpAddress;

        [JsonProperty("method")]
        public string Method;
    }
}
