using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.CognitiveServices
{
    [Export(typeof(IAction))]
    public class DeployCognitiveService : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var cognitiveServiceKey = request.DataStore.GetValue("CognitiveServiceKey");

            if (!string.IsNullOrEmpty(cognitiveServiceKey))
            {
                return new ActionResponse(ActionStatus.Success);
            }

            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var cognitiveServiceName = request.DataStore.GetValue("CognitiveServiceName");
            var cognitiveServiceType = request.DataStore.GetValue("CognitiveServiceType");
            var skuName = request.DataStore.GetValue("CognitiveSkuName");

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("CognitiveServiceName", cognitiveServiceName);
            param.AddStringParam("CognitiveServiceType", cognitiveServiceType);

            if (cognitiveServiceType == "Bing.Search.v7")
            {
                param.AddStringParam("Location", "global");
            }
            else
            {
                param.AddStringParam("Location", "West US");
            }

            param.AddStringParam("skuName", skuName);


            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            var registeration = await client.Providers.RegisterAsync("Microsoft.CognitiveServices");

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/AzureArm/CognitiveServices.json")));
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

            while (true)
            {
                Thread.Sleep(5000);
                var status = client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken()).Result;
                var operations = client.DeploymentOperations.ListAsync(resourceGroup, deploymentName, new DeploymentOperationsListParameters(), new CancellationToken()).Result;
                var provisioningState = status.Deployment.Properties.ProvisioningState;

                if (provisioningState == "Accepted" || provisioningState == "Running")
                { continue; }

                if (provisioningState == "Succeeded")
                {

                    request.Logger.LogResource(request.DataStore, cognitiveServiceName,
                        DeployedResourceType.CognitiveServices, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), cognitiveServiceType, skuName);
                    return new ActionResponse(ActionStatus.Success, operations);
                }

                var operation = operations.Operations.First(p => p.Properties.ProvisioningState == ProvisioningState.Failed);
                var operationFailed = client.DeploymentOperations.GetAsync(resourceGroup, deploymentName, operation.OperationId, new CancellationToken()).Result;

                return new ActionResponse(ActionStatus.Failure, operationFailed);
            }

        }
    }
}