namespace RedditCore.AzureML
{
    public class AzureMLResult
    {
        /// <summary>
        ///     True if the job ran successfully, false if there was a failure, or null
        ///     if the job was sucessfully submitted but we were asked not to wait for completion.
        /// </summary>
        public bool? Success { get; internal set; }

        public string Message { get; internal set; }

        public string Error { get; internal set; }

        public string Details { get; internal set; }

        /// <summary>
        /// Gets the ID of the AzureML job
        /// </summary>
        public string JobId { get; internal set; }

        /// <summary>
        /// Gets the last job status of the AzureML job.  This may not be the final status of the job.  This is the last status that the experiment waiter code saw.
        /// </summary>
        public BatchScoreStatusCode LastJobStatus { get; internal set; }
    }
}
