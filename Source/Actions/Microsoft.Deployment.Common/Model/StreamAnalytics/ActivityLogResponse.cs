using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class ActivityLogResponse
    {
        [JsonProperty("value")]
        public List<ActivityLogEntry> Value;

        [JsonProperty("nextLink")]
        public string NextLink;
    }
}