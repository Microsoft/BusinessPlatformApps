using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployStorageAccountContainer : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var deploymentName = request.DataStore.GetValue("DeploymentName");

            var storageAccountName = request.DataStore.GetValue("StorageAccountName");
            var storageAccountKey= request.DataStore.GetValue("StorageAccountKey");
            var containerName = request.DataStore.GetValue("StorageAccountContainer");

            //var connectionString = $"DefaultEndpoinsProtocol=[https;AccountName={storageAccountName};AccountKey={storageAccountKey}";
            var accountCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount account = new CloudStorageAccount(accountCredentials, false);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            await container.CreateIfNotExistsAsync();

            // Change Container from private to public
            BlobContainerPermissions permissions = container.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            container.SetPermissions(permissions);


            return new ActionResponse(ActionStatus.Success);
        }
    }
}
