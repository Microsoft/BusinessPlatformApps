using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppSqlConnectionUpdateProperties
    {
        [JsonProperty("reference")]
        public string Reference = "cbe235ee-eb8b-5329-1c73-812825f07146";
        [JsonProperty("resourceId")]
        public string ResourceId;

        public PowerAppSqlConnectionUpdateProperties(string name)
        {
            ResourceId = $"/providers/Microsoft.PowerApps/apps/{name}";
        }
    }
}