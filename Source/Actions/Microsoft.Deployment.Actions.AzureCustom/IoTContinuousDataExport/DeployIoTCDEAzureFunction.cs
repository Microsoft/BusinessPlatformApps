﻿using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.IoTContinuousDataExport
{
    [Export(typeof(IAction))]
    public class DeployIoTCDEAzureFunction : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string idSubscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string nameResourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string tokenAzure = request.DataStore.GetJson("AzureToken", "access_token");

            string databaseConnectionString = request.DataStore.GetValue("SqlConnectionString");

            string deploymentName = request.DataStore.GetValue("DeploymentName") ?? request.DataStore.CurrentRoute;

            var armParameters = request.DataStore.GetJson("AzureArmParameters");
            var armParametersUniquePrefix = request.DataStore.GetJson("AzureArmParametersUniquePrefix");

            var payload = new AzureArmParameterGenerator();

            if (armParameters != null)
            {
                foreach (var prop in armParameters.Children())
                {
                    string key = prop.Path.Split('.').Last();
                    string value = prop.First().ToString();

                    payload.AddStringParam(key, value);
                }
            }

            var uniqueValue = request.DataStore.GetValue("UniquePrefix", DataStoreType.Private);
            if (string.IsNullOrWhiteSpace(uniqueValue))
            {
                var resourceManagerId = $"/subscriptions/{idSubscription}/resourceGroups/{nameResourceGroup}";
                uniqueValue = GetUniqueString(resourceManagerId);
                request.DataStore.AddToDataStore("UniquePrefix", uniqueValue, DataStoreType.Private);
            }

            if (armParametersUniquePrefix != null)
            {
                foreach (var prop in armParametersUniquePrefix.Children())
                {
                    string key = prop.Path.Split('.').Last();
                    string value = prop.First().ToString();

                    var valueUnique = $"{uniqueValue}{value}";
                    payload.AddStringParam(key, valueUnique);

                    request.DataStore.AddToDataStore(key, valueUnique, DataStoreType.Private);
                }
            }

            payload.AddStringParam("databaseConnectionString", databaseConnectionString);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, request.DataStore.GetValue("AzureArmFile"))));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(payload.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            ResourceManagementClient client = new ResourceManagementClient(new TokenCloudCredentials(idSubscription, tokenAzure));

            var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            var validate = await client.Deployments.ValidateAsync(nameResourceGroup, deploymentName, deployment, new CancellationToken());
            if (!validate.IsValid)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null, DefaultErrorCodes.DefaultErrorCode,
                    $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = await client.Deployments.CreateOrUpdateAsync(nameResourceGroup, deploymentName, deployment, new CancellationToken());

            ActionResponse r = await WaitForAction(client, nameResourceGroup, deploymentName);
            request.DataStore.AddToDataStore("ArmOutput", r.DataStore.GetValue("ArmOutput"), DataStoreType.Public);
            return r;
        }

        private static string GetUniqueString(string id, int length = 13)
        {
            string result = "";
            var buffer = System.Text.Encoding.UTF8.GetBytes(id);
            var hashArray = new System.Security.Cryptography.SHA512Managed().ComputeHash(buffer);
            for (int i = 1; i <= length; i++)
            {
                var b = hashArray[i];
                var c = Convert.ToChar((b % 26) + (byte)'a');
                result = result + c;
            }

            return result;
        }

        private static async Task<ActionResponse> WaitForAction(ResourceManagementClient client, string resourceGroup, string deploymentName)
        {
            for (; ; )
            {
                Thread.Sleep(Constants.ACTION_WAIT_INTERVAL);
                var status = await client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken());
                var operations = await client.DeploymentOperations.ListAsync(resourceGroup, deploymentName, new DeploymentOperationsListParameters(), new CancellationToken());
                var provisioningState = status.Deployment.Properties.ProvisioningState;

                if (provisioningState == "Accepted" || provisioningState == "Running")
                    continue;

                if (provisioningState == "Succeeded")
                {
                    ActionResponse r = new ActionResponse(ActionStatus.Success, operations) { DataStore = new DataStore() };
                    string outputs = status.Deployment.Properties.Outputs;
                    if (!string.IsNullOrEmpty(outputs))
                        r.DataStore.AddToDataStore("ArmOutput", outputs, DataStoreType.Public);

                    return r;
                }

                var operation = operations.Operations.First(p => p.Properties.ProvisioningState == ProvisioningState.Failed);
                var operationFailed = await client.DeploymentOperations.GetAsync(resourceGroup, deploymentName, operation.OperationId, new CancellationToken());

                return new ActionResponse(ActionStatus.Failure, operationFailed);
            }
        }
    }
}