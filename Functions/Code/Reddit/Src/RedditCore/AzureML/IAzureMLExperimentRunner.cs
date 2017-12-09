using System.Threading.Tasks;

namespace RedditCore.AzureML
{
    public interface IAzureMLExperimentRunner
    {
        Task<AzureMLResult> RunExperimentWithData(string data, bool waitForCompletion);
    }
}
