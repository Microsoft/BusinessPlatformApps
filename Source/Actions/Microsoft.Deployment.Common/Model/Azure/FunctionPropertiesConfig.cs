using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionPropertiesConfig
    {
        [JsonProperty("bindings")]
        public List<FunctionPropertiesConfigBinding> Bindings;
        [JsonProperty("disabled")]
        public bool Disabled;
    }
}