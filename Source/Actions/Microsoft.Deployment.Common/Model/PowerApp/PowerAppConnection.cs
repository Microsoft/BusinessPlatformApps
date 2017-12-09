using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppConnection
    {
        [JsonProperty("datasources")]
        public string[] Datasources = new string[] {
          "[pbist_twitter].[twitter_query]",
          "[pbist_twitter].[twitter_query_details]",
          "[pbist_twitter].[twitter_query_readable]"
        };
        [JsonProperty("dependencies")]
        public string[] Dependencies = new string[] { };
        [JsonProperty("id")]
        public string Id = "/providers/microsoft.powerapps/apis/shared_sql";
        [JsonProperty("isOnPremiseConnection")]
        public bool IsOnPremiseConnection = false;
        [JsonProperty("parameterHints")]
        public object ParameterHints = null;
        [JsonProperty("sharedConnectionId")]
        public string SharedConnectionId;

        public PowerAppConnection(string sharedConnectionId)
        {
            SharedConnectionId = $"/providers/microsoft.powerapps/apis/shared_sql/connections/{sharedConnectionId}";
        }
    }
}