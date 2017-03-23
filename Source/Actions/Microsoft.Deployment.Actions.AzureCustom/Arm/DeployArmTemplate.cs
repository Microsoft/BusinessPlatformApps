//-----------------------------------------------------------------------
// <copyright file="DeployArmTemplate.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.Arm
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using Microsoft.Azure;
    using Microsoft.Azure.Management.Resources;
    using Microsoft.Azure.Management.Resources.Models;
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.ErrorCode;
    using Microsoft.Deployment.Common.Helpers;


    [Export(typeof(IAction))]
    public class DeployArmTemplate : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var deploymentName = request.DataStore.GetValue("DeploymentName");

            // Read from file
            var armTemplate = request.DataStore.GetValue("ArmTemplate");
            var armParamTemplate = request.DataStore.GetValue("ArmParamTemplate");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);

            var deployment = new Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate,
                    Parameters = armParamTemplate
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

            return new ActionResponse(ActionStatus.Success, deploymentItem);
        }
    }
}