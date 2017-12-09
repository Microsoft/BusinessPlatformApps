using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementServiceProperties
    {
        [JsonProperty("addresserEmail")]
        public string AddresserEmail;
        [JsonProperty("createdAtUtc")]
        public string CreatedAtUtc;
        [JsonProperty("managementApiUrl")]
        public string ManagementApiUrl;
        [JsonProperty("portalUrl")]
        public string PortalUrl;
        [JsonProperty("provisioningState")]
        public string ProvisioningState;
        [JsonProperty("publisherEmail")]
        public string PublisherEmail;
        [JsonProperty("publisherName")]
        public string PublisherName;
        [JsonProperty("runtimeUrl")]
        public string RuntimeUrl;
        [JsonProperty("scmUrl")]
        public string ScmUrl;
        [JsonProperty("staticIPs")]
        public List<string> StaticIps;
        [JsonProperty("targetProvisioningState")]
        public string TargetProvisioningState;
        [JsonProperty("vpnType")]
        public int VpnType;
    }
}