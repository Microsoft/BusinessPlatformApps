using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionPropertiesConfigBinding
    {
        [JsonProperty("direction")]
        public string Direction;
        [JsonProperty("authLevel")]
        public string AuthLevel;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("type")]
        public string Type;
    }
}