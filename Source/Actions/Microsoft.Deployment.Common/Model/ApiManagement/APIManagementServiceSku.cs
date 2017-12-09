using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ApiManagement
{
    public class ApiManagementServiceSku
    {
        [JsonProperty("capacity")]
        public int Capacity;
        [JsonProperty("name")]
        public string Name;
    }
}