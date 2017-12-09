using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppPublish
    {
        [JsonProperty("properties")]
        public PowerAppPublishProperties Properties;
        [JsonProperty("tags")]
        public PowerAppPublishTags Tags = new PowerAppPublishTags();

        public PowerAppPublish(string appUri, string displayName, string environmentId, string sqlConnectionId)
        {
            Properties = new PowerAppPublishProperties(appUri, displayName, environmentId, sqlConnectionId);
        }
    }
}