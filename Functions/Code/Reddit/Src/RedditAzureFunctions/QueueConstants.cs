namespace RedditAzureFunctions
{
    public class QueueConstants
    {
        public const string RedditSearchQueueName = "%RedditSearchQueueName%";
        public const string RedditPostQueueName = "%" + RedditPostQueueNameProperty + "%";
        public const string RedditPostQueueNameProperty = "RedditPostQueueName";
        public const string AzureMLJobQueueName = "%" + AzureMLJobQueueNameProperty + "%";
        public const string AzureMLJobQueueNameProperty = "AzureMLJobQueueName";

        public const string QueueConnectionStringName = "StorageQueueConnection";
    }
}