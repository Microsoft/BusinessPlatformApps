using System.Collections.Generic;

namespace RedditCore.AzureML
{
    internal class BatchScoreStatus
    {
        // Status code for the batch scoring job
        public BatchScoreStatusCode StatusCode { get; set; }

        // Locations for the potential multiple batch scoring outputs
        public IDictionary<string, object> Results { get; set; }

        // Error details, if any
        public string Details { get; set; }
    }
}
