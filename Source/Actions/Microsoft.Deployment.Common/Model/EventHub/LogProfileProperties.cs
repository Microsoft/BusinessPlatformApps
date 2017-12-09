using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.EventHub
{
    public class LogProfileProperties
    {
        [JsonProperty("categories")]
        public List<string> Categories;
        [JsonProperty("locations")]
        public List<string> Locations;
        [JsonProperty("serviceBusRuleId")]
        public string ServiceBusRuleId;
        [JsonProperty("storageAccountId")]
        public string StorageAccountId;
    }
}