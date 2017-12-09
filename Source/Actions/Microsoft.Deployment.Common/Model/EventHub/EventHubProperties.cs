using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Deployment.Common.Model.EventHub
{
    public class EventHubProperties
    {
        [JsonProperty("messageRetentionInDays")]
        public int MessageRetention;

        [JsonProperty("status")]
        public string Status;

        [JsonProperty("createdAt")]
        public DateTime? CreatedAt;

        [JsonProperty("updatedAt")]
        public DateTime? UpdatedAt;

        [JsonProperty("partitionCount")]
        public int Properties;

        [JsonProperty("partitionIds")]
        public List<string> PartitionIds; 
    }
}
