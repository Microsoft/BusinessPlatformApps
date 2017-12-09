using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ActivityLog
{
    public class ActivityLogProfileProperties
    {
        [JsonProperty("categories")]
        List<string> Categories = new List<string>() {
            "Write",
            "Delete",
            "Action"
        };
        [JsonProperty("locations")]
        List<string> Locations = new List<string>() {
            "australiaeast",
            "australiasoutheast",
            "brazilsouth",
            "canadacentral",
            "canadaeast",
            "centralindia",
            "centralus",
            "eastasia",
            "eastus",
            "eastus2",
            "japaneast",
            "japanwest",
            "koreacentral",
            "koreasouth",
            "northcentralus",
            "northeurope",
            "southcentralus",
            "southindia",
            "southeastasia",
            "uksouth",
            "ukwest",
            "westcentralus",
            "westeurope",
            "westindia",
            "westus",
            "westus2",
            "global"
        };
        [JsonProperty("retentionPolicy")]
        public ActivityLogProfileRetentionPolicy RetentionPolicy = new ActivityLogProfileRetentionPolicy();
        [JsonProperty("serviceBusRuleId")]
        public string ServiceBusRuleId;
        [JsonProperty("storageAccountId")]
        public string StorageAccountId;

        public ActivityLogProfileProperties(string idSubscription, string nameResourceGroup, string nameNamespace)
        {
            ServiceBusRuleId = $"/subscriptions/{idSubscription}/resourceGroups/{nameResourceGroup}/providers/Microsoft.EventHub/namespaces/{nameNamespace}/authorizationrules/RootManageSharedAccessKey";
        }
    }
}