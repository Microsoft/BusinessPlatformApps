using System;

using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Azure
{
    public class FunctionStatusProperties
    {
        [JsonProperty("active")]
        public bool Active;
        [JsonProperty("author")]
        public string Author;
        [JsonProperty("author_email")]
        public string AuthorEmail;
        [JsonProperty("complete")]
        public bool Complete;
        [JsonProperty("deployer")]
        public string Deployer;
        [JsonProperty("end_time")]
        public DateTime? EndTime;
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("is_readonly")]
        public bool IsReadOnly;
        [JsonProperty("is_temp")]
        public bool IsTemp;
        [JsonProperty("last_success_end_time")]
        public string LastSuccessEndTime;
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("log_url")]
        public string LogUrl;
        [JsonProperty("progress")]
        public string Progress;
        [JsonProperty("received_time")]
        public DateTime? ReceivedTime;
        [JsonProperty("site_name")]
        public string SiteName;
        [JsonProperty("start_time")]
        public DateTime? StartTime;
        [JsonProperty("status")]
        public int Status;
        [JsonProperty("status_text")]
        public string StatusText;
        [JsonProperty("url")]
        public string Url;
    }
}