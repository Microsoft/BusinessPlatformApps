using System;
using Microsoft.Azure.WebJobs.Host;
using RedditCore.Logging;

namespace RedditAzureFunctions.Logging
{
    internal class FunctionLog : ILog
    {
        private readonly TraceWriter traceWriter;
        private readonly Guid invocationId;

        internal FunctionLog(TraceWriter traceWriter, Guid invocationId)
        {
            this.traceWriter = traceWriter;
            this.invocationId = invocationId;
        }

        public void Error(string message, Exception ex = null, string source = null)
        {
            traceWriter.Error(GetMessage(message), ex, source);
        }

        public void Info(string message, string source = null)
        {
            traceWriter.Info(GetMessage(message), source);
        }

        public void Verbose(string message, string source = null)
        {
            traceWriter.Verbose(GetMessage(message), source);
        }

        public void Warning(string message, string source = null)
        {
            traceWriter.Warning(GetMessage(message), source);
        }

        private string GetMessage(string message)
        {
            return $"[Invocation {invocationId}] {message}";
        }
    }
}
