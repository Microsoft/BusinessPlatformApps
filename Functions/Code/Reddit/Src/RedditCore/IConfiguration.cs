using System;

namespace RedditCore
{
    public interface IConfiguration
    {
        string AzureMLApiKey { get; }

        string AzureMLBaseUrl { get; }

        string DbConnectionString { get; }

        string SocialGistApiKey { get; }

        TimeSpan SocialGistApiRequestTimeout { get; }

        TimeSpan SearchToThreadTimeDelay { get; }

        TimeSpan AzureMlRetryTimeDelay { get; }

        string QueryTerms { get; }

        // ReSharper disable once BuiltInTypeReferenceStyle
        int MaximumResultsPerSearch { get; }

        // ReSharper disable once BuiltInTypeReferenceStyle
        int ResultLimitPerPage { get; }

        string ObjectLogSASUrl { get; }
        string ObjectLogBlobContainer { get; }
        bool UseObjectLogger { get; }

        string RedditPostQueueName { get; }

        string AzureMLJobQueueName { get; }

        Guid FunctionInvocationId { get; }

        string FunctionName { get; }

        bool IngestOnlyDocumentsWithUserDefinedEntities { get; }
    }
}