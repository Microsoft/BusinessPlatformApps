using System;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppEnvironmentProperties
    {
        [JsonProperty("azureRegionHint")]
        public string AzureRegionHint;
        [JsonProperty("createdBy")]
        public PowerAppEnvironmentPropertiesUser CreatedBy;
        [JsonProperty("createdTime")]
        public DateTime createdTime;
        [JsonProperty("creationType")]
        public string CreationType;
        [JsonProperty("displayName")]
        public string DisplayName;
        [JsonProperty("environmentSku")]
        public string EnvironmentSku;
        [JsonProperty("environmentType")]
        public string EnvironmentType;
        [JsonProperty("isDefault")]
        public bool IsDefault;
        [JsonProperty("lastModifiedBy")]
        public PowerAppEnvironmentPropertiesUser LastModifiedBy;
        [JsonProperty("lastModifiedTime")]
        public DateTime lastModifiedTime;
        [JsonProperty("permissions")]
        public PowerAppEnvironmentPropertiesPermissions Permissions;
        [JsonProperty("provisioningState")]
        public string ProvisioningState;
    }
}