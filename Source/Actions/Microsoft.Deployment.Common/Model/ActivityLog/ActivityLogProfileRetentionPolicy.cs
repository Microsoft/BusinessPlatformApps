using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.ActivityLog
{
    public class ActivityLogProfileRetentionPolicy
    {
        [JsonProperty("enabled")]
        public bool Enabled = false;
        [JsonProperty("days")]
        public int Days = 0;
    }
}