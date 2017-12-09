using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppSqlConnectionUpdate
    {
        [JsonProperty("properties")]
        public PowerAppSqlConnectionUpdateProperties Properties;

        public PowerAppSqlConnectionUpdate(string name)
        {
            Properties = new PowerAppSqlConnectionUpdateProperties(name);
        }
    }
}