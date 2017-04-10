using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Informatica
{
#pragma warning disable 649
    class InformaticaRuntimeEnvironment : InformaticaObject
    {
        [JsonProperty("orgId")]
        public string OrgId;

        [JsonProperty("agents")]
        public InformaticaAgent[] Agents;

        public InformaticaRuntimeEnvironment()
        {
            Type = "runtimeEnvironment";
        }
    }
#pragma warning restore 649
}