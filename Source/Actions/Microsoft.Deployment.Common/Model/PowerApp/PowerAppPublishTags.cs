using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppPublishTags
    {
        [JsonProperty("deviceCapabilities")]
        public string DeviceCapabilities = string.Empty;
        [JsonProperty("minimumRequiredApiVersion")]
        public string MinimumRequiredApiVersion = "2.2.0";
        [JsonProperty("publisherVersion")]
        public string PublisherVersion = "2.0.720";
        [JsonProperty("primaryDeviceHeight")]
        public string PrimaryDeviceHeight = "768";
        [JsonProperty("primaryDeviceWidth")]
        public string PrimaryDeviceWidth = "1366";
        [JsonProperty("primaryFormFactor")]
        public string PrimaryFormFactor = "Tablet";
        //[JsonProperty("sienaVersion")]
        //public string SienaVersion = "";
        [JsonProperty("supportsLandscape")]
        public string SupportsLandscape = "true";
        [JsonProperty("supportsPortrait")]
        public string SupportsPortrait = "false";
    }
}