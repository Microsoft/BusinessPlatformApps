using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppDocumentUri
    {
        [JsonProperty("value")]
        public string Value;

        public PowerAppDocumentUri(string value)
        {
            Value = value;
        }
    }
}