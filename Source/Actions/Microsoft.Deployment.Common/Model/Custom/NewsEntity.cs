using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Custom
{
    public class NewsEntity
    {
        [JsonProperty("color")]
        public string Color;
        [JsonProperty("icon")]
        public string Icon;
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("values")]
        public string Values;
    }
}
