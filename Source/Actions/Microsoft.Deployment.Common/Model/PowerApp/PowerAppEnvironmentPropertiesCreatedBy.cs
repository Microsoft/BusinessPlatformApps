using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppEnvironmentPropertiesUser
    {
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("email")]
        public string Email;
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("tenantId")]
        public string TenantId;
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName;
    }
}