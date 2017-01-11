//-----------------------------------------------------------------------
// <copyright file="DeployAzureMLWorkspace.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class DeployAzureMLWorkspace : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var deploymentName = request.DataStore.GetValue("DeploymentName") ?? "AzureMLDeployment";

            var workspaceName = request.DataStore.GetValue("WorkspaceName");
            var storageAccountName = request.DataStore.GetValue("StorageAccountName");
            var planName = request.DataStore.GetValue("PlanName") ?? "testazuremlplan";
            var skuName = request.DataStore.GetValue("SkuName")?? "S1";
            var skuTier = request.DataStore.GetValue("SkuTier") ?? "Standard";
            var skuCapacity = request.DataStore.GetValue("SkuCapacity") ?? "1";

            // Get email address

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("name", workspaceName);
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("newStorageAccountName", storageAccountName);
            param.AddStringParam("planName", planName);
            param.AddStringParam("skuName", skuName);
            param.AddStringParam("skuTier", skuTier);
            param.AddStringParam("skuCapacity", skuCapacity);
            param.AddStringParam("ownerEmail", AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken")));

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, "Service/Arm/azureml.json")));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);


            var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            var validate = client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;
            if (!validate.IsValid)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null,
                     DefaultErrorCodes.DefaultErrorCode, $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;

            return new ActionResponse(ActionStatus.Success);
        }
    }
}

