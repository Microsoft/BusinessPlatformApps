using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    public class SignalExecuteRedditSearchFunction : BaseAction
    {
        public const string StorageAccountConnectionStringKey = "StorageAccountConnectionString";
        public const string StorageQueueNameKey = "StorageQueueName";

        public static readonly CloudQueueMessage BeginMessage = new CloudQueueMessage("begin"); 

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var storageAccountConnectionString = request.DataStore.GetValue(StorageAccountConnectionStringKey);
            var storageQueueName = request.DataStore.GetValue(StorageQueueNameKey);

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(storageAccountConnectionString))
            {
                errors.Add($"{StorageAccountConnectionStringKey} not defined");
            }

            if (string.IsNullOrWhiteSpace(storageQueueName))
            {
                errors.Add($"{StorageQueueNameKey} not defined");
            }

            if (errors.Count != 0)
            {
                var errorMessage = string.Join("<br/>", errors);
                return new ActionResponse(
                    ActionStatus.Failure,
                    errors,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    errorMessage
                );
            }

            if (!CloudStorageAccount.TryParse(storageAccountConnectionString, out var cloudStorageAccount))
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"Azure storage account was not resolvable.  This is required to start function processing"
                );
            }

            var queueClient = cloudStorageAccount.CreateCloudQueueClient();
            var cloudQueue = queueClient.GetQueueReference(storageQueueName);
            cloudQueue.CreateIfNotExists();
            cloudQueue.AddMessage(BeginMessage);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}