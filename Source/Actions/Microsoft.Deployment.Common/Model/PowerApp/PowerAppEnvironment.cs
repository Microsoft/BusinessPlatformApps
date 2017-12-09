using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppEnvironment
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("location")]
        public string Location;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public PowerAppEnvironmentProperties Properties;
        [JsonProperty("type")]
        public string Type;

        public PowerAppEnvironment(string environmentId)
        {
            Id = $"/providers/Microsoft.PowerApps/environments/{environmentId}";
            Name = environmentId;
        }
    }
}