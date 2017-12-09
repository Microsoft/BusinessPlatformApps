using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.EventHub
{
    public class LogProfile
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("location")]
        public string Location;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public LogProfileProperties Properties;
        [JsonProperty("type")]
        public string Type;
    }
}