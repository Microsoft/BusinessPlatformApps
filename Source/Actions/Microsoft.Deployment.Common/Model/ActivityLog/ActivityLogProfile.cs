using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ActivityLog
{
    public class ActivityLogProfile
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("location")]
        public string Location;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("properties")]
        public ActivityLogProfileProperties Properties;
        [JsonProperty("tags")]
        public string Tags;

        public ActivityLogProfile(string idSubscription, string nameResourceGroup, string nameNamespace)
        {
            Properties = new ActivityLogProfileProperties(idSubscription, nameResourceGroup, nameNamespace);
        }
    }
}