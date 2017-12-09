using System;

namespace RedditCore.AzureML
{
    public class ScheduledAzureMLProcessorException : Exception
    {
        public ScheduledAzureMLProcessorException()
            : base()
        {

        }

        public ScheduledAzureMLProcessorException(string message)
            : base(message)
        {

        }
    }
}
