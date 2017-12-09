using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.EventHub
{
    public class EventHub
    {
        [JsonProperty("id")]
        public string Id;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("type")]
        public string Type;

        [JsonProperty("properties")]
        public EventHubProperties Properties;
    }
}
