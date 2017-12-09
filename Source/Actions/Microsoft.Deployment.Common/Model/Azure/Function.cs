using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class Function
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("location")]
        public string Location;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public FunctionProperties Properties;
        [JsonProperty("type")]
        public string Type;
    }
}