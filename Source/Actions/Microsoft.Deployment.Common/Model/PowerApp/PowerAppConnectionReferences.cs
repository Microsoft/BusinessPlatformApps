using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppConnectionReferences
    {
        [JsonProperty("cbe235ee-eb8b-5329-1c73-812825f07146")]
        public PowerAppConnection Connection;
        
        public PowerAppConnectionReferences(string sqlConnectionId)
        {
            Connection = new PowerAppConnection(sqlConnectionId);
        }
    }
}