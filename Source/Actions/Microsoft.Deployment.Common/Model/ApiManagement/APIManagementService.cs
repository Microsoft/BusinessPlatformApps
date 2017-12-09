using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementService
    {
        [JsonProperty("etag")]
        public string ETag;
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("location")]
        public string Location;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public ApiManagementServiceProperties Properties;
        [JsonProperty("sku")]
        public ApiManagementServiceSku Sku;
        [JsonProperty("type")]
        public string Type;
    }
}