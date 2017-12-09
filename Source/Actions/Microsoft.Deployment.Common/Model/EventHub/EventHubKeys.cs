using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.EventHub
{
    public class EventHubKeys
    {
        [JsonProperty("keyName")]
        public string KeyName;
        [JsonProperty("primaryConnectionString")]
        public string PrimaryConnectionString;
        [JsonProperty("primaryKey")]
        public string PrimaryKey;
        [JsonProperty("secondaryConnectionString")]
        public string SecondaryConnectionString;
        [JsonProperty("secondaryKey")]
        public string SecondaryKey;
    }
}