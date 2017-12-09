using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppPublishProperties
    {
        [JsonProperty("appUris")]
        public PowerAppUris AppUris;
        [JsonProperty("backgroundColor")]
        public string BackgroundColor = "RGBA(0,176,240,1)";
        [JsonProperty("backgroundImageUri")]
        public string BackgroundImageUri = string.Empty;
        [JsonProperty("connectionReferences")]
        public PowerAppConnectionReferences ConnectionReferences;
        [JsonProperty("createdByClientVersion")]
        public PowerAppVersion CreatedByClientVersion = new PowerAppVersion();
        [JsonProperty("description")]
        public string Description = string.Empty;
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("environment")]
        public PowerAppEnvironment Environment;
        [JsonProperty("lifeCycleId")]
        public string LifeCycleId = "Draft";
        [JsonProperty("minClientVersion")]
        public PowerAppVersion MinClientVersion = new PowerAppVersion();

        public PowerAppPublishProperties(string appUri, string displayName, string environmentId, string sqlConnectionId)
        {
            AppUris = new PowerAppUris(appUri);
            ConnectionReferences = new PowerAppConnectionReferences(sqlConnectionId);
            DisplayName = displayName;
            Environment = new PowerAppEnvironment(environmentId);
        }
    }
}