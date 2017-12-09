using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppSqlConnectionProperties
    {
        [JsonProperty("connectionParameters")]
        public PowerAppSqlConnectionPropertiesConnectionParameters ConnectionParameters;
        [JsonProperty("environment")]
        public PowerAppEnvironment Environment;

        public PowerAppSqlConnectionProperties(SqlCredentials sqlCredentials, string environmentId)
        {
            ConnectionParameters = new PowerAppSqlConnectionPropertiesConnectionParameters(sqlCredentials);
            Environment = new PowerAppEnvironment(environmentId);
        }
    }
}