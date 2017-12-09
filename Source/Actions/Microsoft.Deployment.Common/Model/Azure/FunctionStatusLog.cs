using System;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionStatusLog
    {
        [JsonProperty("details_url")]
        public string DetailsUrl;
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("log_time")]
        public DateTime? LogTime;
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("type")]
        public int Type;
    }
}