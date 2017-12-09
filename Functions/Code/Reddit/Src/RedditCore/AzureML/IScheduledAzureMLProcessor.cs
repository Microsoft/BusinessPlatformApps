using System.Threading.Tasks;

namespace RedditCore.AzureML
{
    public interface IScheduledAzureMLProcessor
    {
        /// <summary>
        /// Start the AzureML job
        /// </summary>
        /// <returns>Result of the job start</returns>
        Task<AzureMLResult> RunAzureMLProcessing();

        /// <summary>
        /// Checks an AzureML job.  If it finishes or is finished then run the post-process cleanup.
        /// </summary>
        /// <param name="jobId">ID of the job to run</param>
        /// <returns>Result of the job run</returns>
        Task<AzureMLResult> CheckAzureMLAndPostProcess(string jobId);
    }
}
