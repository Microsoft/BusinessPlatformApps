using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppResourceStorage
    {
        [JsonProperty("sharedAccessSignature")]
        public string SharedAccessSignature;
    }
}