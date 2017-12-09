using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.ErrorCode;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Common.Helpers
{
    public class DeploymentHelper
    {
        public async Task<ActionResponse> DeployLogicApp(string subscription, string azureToken, string resourceGroup, JObject armTemplate, string deploymentName)
        {
            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);

            var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            var validate = await client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());

            if (!validate.IsValid)
                return new ActionResponse(
                    ActionStatus.Failure,
                    JsonUtility.GetJObjectFromObject(validate),
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");

            var deploymentItem = await client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());

            var resp = await WaitForDeployment(client, resourceGroup, deploymentItem.Deployment.Name);

            if (!resp.IsSuccess)
            {
                return resp;
            }

            return await WaitForDeployment(client, resourceGroup, deploymentName);
        }

        public async Task<ActionResponse> WaitForDeployment(ResourceManagementClient client, string resourceGroup, string deploymentName)
        {
            while (true)
            {
                Thread.Sleep(5000);
                var status = await client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken());
                var operations =
                    await
                        client.DeploymentOperations.ListAsync(resourceGroup, deploymentName,
                            new DeploymentOperationsListParameters(), new CancellationToken());
                var provisioningState = status.Deployment.Properties.ProvisioningState;

                if (provisioningState == "Accepted" || provisioningState == "Running")
                    continue;

                if (provisioningState == "Succeeded")
                    return new ActionResponse(ActionStatus.Success, operations);

                var operation = operations.Operations.First(p => p.Properties.ProvisioningState == ProvisioningState.Failed);
                var operationFailed =
                    await
                        client.DeploymentOperations.GetAsync(resourceGroup, deploymentName, operation.OperationId,
                            new CancellationToken());

                return new ActionResponse(ActionStatus.Failure, operationFailed);
            }
        }
    }
}
