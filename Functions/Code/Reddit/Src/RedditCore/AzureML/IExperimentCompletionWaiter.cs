using System;
using System.Threading.Tasks;

namespace RedditCore.AzureML
{
    public interface IExperimentCompletionWaiter
    {
        /// <summary>
        /// Waits for a job to complete.  If the job does not complete after 10 minutes then the job will be killed.
        /// </summary>
        /// <param name="jobId">ID of the job to wait for</param>
        /// <returns>Result of the job run</returns>
        Task<AzureMLResult> WaitForJobCompletion(string jobId);

        /// <summary>
        /// Waits for a job to complete.
        /// </summary>
        /// <param name="jobId">ID of the job to wait for</param>
        /// <param name="timeout">Time to wait for the job to complete</param>
        /// <param name="killJobAfterTimeout">If true after the timeout has elapsed then the job will be killed.  If false after the timeout has elapsed then the function will exit and the job will be left running.</param>
        /// <returns>Result of the job run</returns>
        Task<AzureMLResult> WaitForJobCompletion(string jobId, TimeSpan timeout, bool killJobAfterTimeout);
    }
}
