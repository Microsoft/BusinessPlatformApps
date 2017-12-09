using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppSqlConnectionPropertiesConnectionParameters
    {
        [JsonProperty("database")]
        public string Database;
        [JsonProperty("password")]
        public string Password;
        [JsonProperty("server")]
        public string Server;
        [JsonProperty("username")]
        public string Username;

        public PowerAppSqlConnectionPropertiesConnectionParameters(SqlCredentials sqlCredentials)
        {
            Database = sqlCredentials.Database;
            Password = sqlCredentials.Password;
            Server = sqlCredentials.Server;
            Username = sqlCredentials.Username;
        }
    }
}