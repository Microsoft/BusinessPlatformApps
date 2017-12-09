using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppUris
    {
        [JsonProperty("documentUri")]
        public PowerAppDocumentUri DocumentUri;

        public PowerAppUris(string uri)
        {
            DocumentUri = new PowerAppDocumentUri(uri);
        }
    }
}