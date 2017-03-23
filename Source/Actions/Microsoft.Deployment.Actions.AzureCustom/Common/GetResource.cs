using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetResource : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var resource = request.DataStore.GetValue("Resource");
            var resourceType = request.DataStore.GetValue("ResourceType");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);

            ResourceIdentity identity = new ResourceIdentity(resource, resourceType, "2015-08-01");
            var resourceFound = await client.Resources.GetAsync(resourceGroup, identity, new CancellationToken());
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromObject(resourceFound.Resource));
        }
    }
}
