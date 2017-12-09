using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionWrapper
    {
        [JsonProperty("value")]
        public List<Function> Value;
    }
}