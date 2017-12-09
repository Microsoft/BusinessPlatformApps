namespace RedditCore.Logging
{
    internal class NoOpObjectLogger : IObjectLogger
    {
        public void Log(object data, string source, string message = null)
        {
            // Do nothing
        }
    }
}
