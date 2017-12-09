using System.Collections.Generic;

namespace RedditCore.AzureML
{
    internal class BatchExecutionRequest
    {
        public IDictionary<string, object> Inputs { get; set; }

        public IDictionary<string, string> GlobalParameters { get; set; }

        // Locations for the potential multiple batch scoring outputs
        public IDictionary<string, object> Outputs { get; set; }
    }
}
