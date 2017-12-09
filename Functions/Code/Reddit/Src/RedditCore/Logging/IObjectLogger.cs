namespace RedditCore.Logging
{
    public interface IObjectLogger
    {
        /// <summary>
        /// Writes data to a log in a permanent store.  This allows us to recreate data at any point in the program.
        /// </summary>
        /// <param name="data">Data to be logged.  Data will be converted to JSON unless it is already a string.</param>
        /// <param name="source">Part of the program that is logging the data.</param>
        /// <param name="message">Additional information about the logged object.</param>
        void Log(object data, string source, string message = null);
    }
}
