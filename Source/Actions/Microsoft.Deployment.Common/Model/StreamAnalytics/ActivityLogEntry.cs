using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class ActivityLogEntry
    {
        [JsonProperty("authorization")]
        public StreamAnalyticsAuthorization Authorization;

        [JsonProperty("caller")]
        public string Caller;

        [JsonProperty("category")]
        public StreamAnalyticsCategory Category;

        [JsonProperty("channels")]
        public string Channels;

        [JsonProperty("claims")]
        public StreamAnalyticsClaims Claims;

        [JsonProperty("correlationId")]
        public string CorrelationId;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("eventDataId")]
        public string EventDataId;

        [JsonProperty("eventName")]
        public StreamAnalyticsEventName EventName;

        [JsonProperty("eventSource")]
        public StreamAnalyticsEventSource EventSource;

        [JsonProperty("httpRequest")]
        public StreamAnalyticsHttpRequest HttpRequest;

        [JsonProperty("id")]
        public string Id;

        [JsonProperty("level")]
        public string Level;

        [JsonProperty("resourceGroupName")]
        public string ResourceGroupName;

        [JsonProperty("resourceProviderName")]
        public StreamAnalyticsResourceProviderName ResourceProviderName;

        [JsonProperty("operationId")]
        public string OperationId;

        [JsonProperty("operationName")]
        public StreamAnalyticsOperationName OperationName;

        [JsonProperty("properties")]
        public StreamAnalyticsProperties Properties;

        [JsonProperty("status")]
        public StreamAnalyticsStatus Status;

        [JsonProperty("subStatus")]
        public StreamAnalyticsSubStatus SubStatus;

        [JsonProperty("eventTimestamp")]
        public string EventTimestamp;

        [JsonProperty("submissionTimestamp")]
        public string SubmissionTimestamp;

        [JsonProperty("subscriptionId")]
        public string SubscriptionId;
    }
}