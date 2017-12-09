using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppConsentScopes
    {
        [JsonProperty("will")]
        public List<string> Will = new List<string>();

        [JsonProperty("wont")]
        public List<string> Wont = new List<string>();
    }
}
