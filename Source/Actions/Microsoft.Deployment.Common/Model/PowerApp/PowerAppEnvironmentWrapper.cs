using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppEnvironmentWrapper
    {
        [JsonProperty("environment")]
        public PowerAppEnvironment Environment;

        public PowerAppEnvironmentWrapper(string environmentId)
        {
            Environment = new PowerAppEnvironment(environmentId);
        }
    }
}