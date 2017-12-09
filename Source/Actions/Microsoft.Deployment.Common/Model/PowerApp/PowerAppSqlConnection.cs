using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppSqlConnection
    {
        [JsonProperty("properties")]
        public PowerAppSqlConnectionProperties Properties;

        public PowerAppSqlConnection(SqlCredentials sqlCredentials, string environmentId)
        {
            Properties = new PowerAppSqlConnectionProperties(sqlCredentials, environmentId);
        }
    }
}