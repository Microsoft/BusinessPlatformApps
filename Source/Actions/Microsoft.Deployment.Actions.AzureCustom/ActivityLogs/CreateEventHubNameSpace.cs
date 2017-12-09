using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class CreateEventHubNameSpace : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string doNotWaitString = request.DataStore.GetValue("Wait");
            bool doNotWait = !string.IsNullOrEmpty(doNotWaitString) && bool.Parse(doNotWaitString);
            string deploymentName = "EHDeployment-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            string namespaceName = "ActivityLogNamespace-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            string eventHubName = "ActivityLogEventHub-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            string consumerGroupName = "ActivityLogConsumerGroup-" + RandomGenerator.GetRandomLowerCaseCharacters(5);
            request.DataStore.AddToDataStore("EHDeployment", deploymentName);
            request.DataStore.AddToDataStore("ActivityLogNamespace", namespaceName);
            request.DataStore.AddToDataStore("ActivityLogEventHub", eventHubName);
            request.DataStore.AddToDataStore("ActivityLogConsumerGroup", consumerGroupName);

            // Read from file
            var armTemplatefilePath = "Service/ARM/EventHub.json";
            var payload = new JObject();
            payload.Add("namespaceName", namespaceName);
            payload.Add("eventHubName", eventHubName);
            payload.Add("consumerGroupName", consumerGroupName);
            var armParamTemplateProperties = payload;

            if (deploymentName == null && !doNotWait)
            {
                deploymentName = request.DataStore.CurrentRoute;
            }

            var param = new AzureArmParameterGenerator();
            foreach (var prop in armParamTemplateProperties.Children())
            {
                string key = prop.Path.Split('.').Last();
                string value = prop.First().ToString();

                param.AddStringParam(key, value);
            }

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, armTemplatefilePath)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

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

            if (doNotWait)
            {
                return new ActionResponse(ActionStatus.Success, deploymentItem);
            }

            return await WaitForAction(client, resourceGroup, deploymentName);
        }

        private static async Task<ActionResponse> WaitForAction(ResourceManagementClient client, string resourceGroup, string deploymentName)
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