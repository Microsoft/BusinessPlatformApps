using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppConsent
    {
        [JsonProperty("connectionId")]
        public string ConnectionId;

        [JsonProperty("consentType")]
        public string ConsentType = "appCreator";

        [JsonProperty("scopes")]
        public PowerAppConsentScopes Scopes = new PowerAppConsentScopes();

        public PowerAppConsent(string sqlConnectionId)
        {
            ConnectionId = "/providers/microsoft.powerapps/apis/shared_sql/connections/" + sqlConnectionId;
        }
    }
}
