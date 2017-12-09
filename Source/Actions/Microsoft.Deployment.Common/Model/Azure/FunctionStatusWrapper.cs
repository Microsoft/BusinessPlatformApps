using System.Collections.Generic;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionStatusWrapper
    {
        [JsonProperty("value")]
        public List<FunctionStatus> Value;
    }
}