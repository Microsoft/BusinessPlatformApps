namespace RedditCore.AzureML
{
    public enum BatchScoreStatusCode
    {
        Unknown,
        NotStarted,
        Running,
        Failed,
        Cancelled,
        Finished
    }
    public static class BatchScoreStatusCodeExtensions
    {
        public static bool IsTerminalState(this BatchScoreStatusCode code)
        {
            return code == BatchScoreStatusCode.Finished
                || code == BatchScoreStatusCode.Failed
                || code == BatchScoreStatusCode.Cancelled;
        }
    }
}
