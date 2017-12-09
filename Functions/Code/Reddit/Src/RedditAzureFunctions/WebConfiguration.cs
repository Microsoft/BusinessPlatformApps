using System.Configuration;
using RedditCore;
using System;
using Microsoft.Azure.WebJobs;

namespace RedditAzureFunctions
{
    internal class WebConfiguration : IConfiguration
    {
        internal WebConfiguration(ExecutionContext context)
        {
            this.FunctionInvocationId = context.InvocationId;
            this.FunctionName = context.FunctionName;
        }

        public string AzureMLApiKey => ConfigurationManager.AppSettings["AzureMLApiKey"];

        public string AzureMLBaseUrl => ConfigurationManager.AppSettings["AzureMLBaseUrl"];

        public string DbConnectionString => ConfigurationManager.ConnectionStrings["SqlConnection"].ConnectionString;

        public string SocialGistApiKey => ConfigurationManager.AppSettings["SocialGistApiKey"];

        public TimeSpan SocialGistApiRequestTimeout =>
            ConfigurationManager.AppSettings["SocialGistApiRequestTimeoutInSeconds"] != null
                ? TimeSpan.FromSeconds(
                    int.Parse(ConfigurationManager.AppSettings["SocialGistApiRequestTimeoutInSeconds"]))
                : TimeSpan.FromSeconds(100); // HttpClient has a default timeout of 100 seconds

        private const string SearchToThreadTimeDelayProperty = "SearchToThreadTimeDelayInSeconds";

        public TimeSpan SearchToThreadTimeDelay =>
            ConfigurationManager.AppSettings[SearchToThreadTimeDelayProperty] != null
                ? TimeSpan.FromSeconds(
                    int.Parse(ConfigurationManager.AppSettings[SearchToThreadTimeDelayProperty]))
                : TimeSpan.FromMinutes(1);

        private const string AzureMlRetryTimeDelayProperty = "AzureMLRetryTimeDelay";

        public TimeSpan AzureMlRetryTimeDelay =>
            ConfigurationManager.AppSettings[AzureMlRetryTimeDelayProperty] != null
                ? TimeSpan.FromSeconds(
                    int.Parse(ConfigurationManager.AppSettings[AzureMlRetryTimeDelayProperty]))
                : TimeSpan.FromSeconds(20);
        
        public string QueryTerms => ConfigurationManager.AppSettings["QueryTerms"];

        // ReSharper disable once BuiltInTypeReferenceStyle
        public int MaximumResultsPerSearch => int.Parse(ConfigurationManager.AppSettings["MaximumResultsPerSearch"]);

        // ReSharper disable once BuiltInTypeReferenceStyle
        public int ResultLimitPerPage => int.Parse(ConfigurationManager.AppSettings["ResultLimitPerPage"]);

        public string ObjectLogSASUrl => ConfigurationManager.AppSettings["ObjectLogSASUrl"];

        public string ObjectLogBlobContainer => GetOrDefault("ObjectLogBlobContainer", "object-logs");

        public bool UseObjectLogger => ConfigurationManager.AppSettings["ObjectLogSASUrl"] != null;

        // Used solely for logging purposes
        public string RedditPostQueueName => ConfigurationManager.AppSettings["RedditPostQueueName"];

        public string AzureMLJobQueueName => ConfigurationManager.AppSettings["AzureMLJobQueueName"];

        public Guid FunctionInvocationId { get; private set; }

        public string FunctionName { get; private set; }

        public bool IngestOnlyDocumentsWithUserDefinedEntities => GetOrDefault("IngestOnlyDocumentsWithUserDefinedEntities", true);

        private static bool GetOrDefault(string property, bool defaultValue)
        {
            if (ConfigurationManager.AppSettings[property] != null)
            {
                return Boolean.Parse(ConfigurationManager.AppSettings[property]);
            }
            else
            {
                return defaultValue;
            }
        }

        private static string GetOrDefault(string property, string defaultValue)
        {
            if (ConfigurationManager.AppSettings[property] != null)
            {
                return ConfigurationManager.AppSettings[property];
            }
            else
            {
                return defaultValue;
            }
        }
    }
}