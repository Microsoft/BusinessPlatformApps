using System;
using System.Diagnostics;

namespace RedditCore.Logging
{
    /// <summary>
    ///     Logs to the console.  This class is useful for writing log information during testing.
    /// </summary>
    public class ConsoleLog : ILog
    {
        public void Error(string message, Exception ex = null, string source = null)
        {
            Console.WriteLine($"{DateTime.Now} ERROR: {message}, Source: {source}, Exception: {ex}");
            Debug.WriteLine($"{DateTime.Now} ERROR: {message}, Source: {source}, Exception: {ex}");
        }

        public void Info(string message, string source = null)
        {
            this.Log("INFO", message, source);
        }

        public void Verbose(string message, string source = null)
        {
            this.Log("VERBOSE", message, source);
        }

        public void Warning(string message, string source = null)
        {
            this.Log("WARNING", message, source);
        }

        private void Log(string severity, string message, string source)
        {
            string output = $"{DateTime.Now} {severity}: {message}, Source: {source}";
            Console.WriteLine(output);
            Debug.WriteLine(output);
        }
    }
}
