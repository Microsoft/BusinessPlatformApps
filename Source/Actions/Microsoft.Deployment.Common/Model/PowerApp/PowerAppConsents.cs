using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppConsents
    {
        [JsonProperty("consents")]
        List<PowerAppConsent> Consents = new List<PowerAppConsent>();

        public PowerAppConsents(string sqlConnectionId)
        {
            Consents.Add(new PowerAppConsent(sqlConnectionId));
        }
    }
}
