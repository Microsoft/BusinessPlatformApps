using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.PowerApp
{
    public class PowerAppVersion
    {
        [JsonProperty("build")]
        public int Build = 720;
        [JsonProperty("major")]
        public int Major = 2;
        [JsonProperty("majorRevision")]
        public int MajorRevision = 0;
        [JsonProperty("minor")]
        public int Minor = 0;
        [JsonProperty("minorRevision")]
        public int MinorRevision = 0;
        [JsonProperty("revision")]
        public int Revision = 0;
    }
}