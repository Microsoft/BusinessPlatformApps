
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Actions.AzureCustom.Arm
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using Microsoft.Azure;
    using Microsoft.Azure.Management.Resources;
    using Microsoft.Deployment.Common.Actions;

    [Export(typeof(IAction))]
    public class GetArmDeploymentStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var deploymentName = request.DataStore.GetValue("DeploymentName");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            var status = await client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken());

            return new ActionResponse(ActionStatus.Success, status);
        }
    }
}