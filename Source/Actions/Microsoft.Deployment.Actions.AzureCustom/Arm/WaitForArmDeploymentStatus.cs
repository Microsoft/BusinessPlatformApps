using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources.Models;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

using ResourceManagementClient = Microsoft.Azure.Management.Resources.ResourceManagementClient;
using Microsoft.Deployment.Common;

namespace Microsoft.Deployment.Actions.AzureCustom.Arm
{

    [Export(typeof(IAction))]
    public class WaitForArmDeploymentStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var deploymentName = request.DataStore.GetValue("DeploymentName");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);

            for(;;)
            {
                Thread.Sleep(Constants.ACTION_WAIT_INTERVAL);
                var status = await client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken());
                var operations = await client.DeploymentOperations.ListAsync(resourceGroup, deploymentName, new DeploymentOperationsListParameters(), new CancellationToken());
                var provisioningState = status.Deployment.Properties.ProvisioningState;

                if (provisioningState == "Accepted" || provisioningState == "Running")
                    continue;

                if (provisioningState == "Succeeded")
                    return new ActionResponse(ActionStatus.Success, operations);

                var operation = operations.Operations.First(p => p.Properties.ProvisioningState == ProvisioningState.Failed);
                var operationFailed = await client.DeploymentOperations.GetAsync(resourceGroup, deploymentName, operation.OperationId, new CancellationToken());

                return new ActionResponse(ActionStatus.Failure, operationFailed);
            }
        }
    }
}