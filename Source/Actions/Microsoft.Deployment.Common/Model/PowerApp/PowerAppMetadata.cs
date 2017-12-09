using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppMetadata
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public PowerAppPublishProperties Properties;
        [JsonProperty("tags")]
        public PowerAppPublishTags Tags;
        [JsonProperty("type")]
        public string Type;
    }
}