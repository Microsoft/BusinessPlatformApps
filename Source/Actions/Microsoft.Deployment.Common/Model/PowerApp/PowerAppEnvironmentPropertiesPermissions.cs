using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppEnvironmentPropertiesPermissions
    {
        [JsonProperty("CreateCustomApi")]
        public PowerAppEnvironmentPropertiesPermission CreateCustomApi;
        [JsonProperty("CreateFlow")]
        public PowerAppEnvironmentPropertiesPermission CreateFlow;
        [JsonProperty("CreateGateway")]
        public PowerAppEnvironmentPropertiesPermission CreateGateway;
        [JsonProperty("CreatePowerApp")]
        public PowerAppEnvironmentPropertiesPermission CreatePowerApp;
        [JsonProperty("GenerateResourceStorage")]
        public PowerAppEnvironmentPropertiesPermission GenerateResourceStorage;
        [JsonProperty("ReadEnvironment")]
        public PowerAppEnvironmentPropertiesPermission ReadEnvironment;
    }
}