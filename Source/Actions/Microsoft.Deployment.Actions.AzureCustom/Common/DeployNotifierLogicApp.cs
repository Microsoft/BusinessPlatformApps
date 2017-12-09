using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Azure;
using System.IO;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using System.Threading;
using Microsoft.Deployment.Common.ErrorCode;
using System.Net.Http;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Microsoft.Deployment.Common.Enums;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployNotifierLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string deploymentName = request.DataStore.GetValue("DeploymentName");
            string logicAppTrigger = string.Empty;
            string logicAppName = request.DataStore.GetValue("logicAppName");
            string sqlConnectionName = request.DataStore.GetValue("sqlConnectionName");

            // Read from file
            var logicAppJsonLocation = "Service/Notifier/notifierLogicApp.json";

            if (deploymentName == null)
            {
                deploymentName = request.DataStore.CurrentRoute;
            }

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("logicAppTrigger", string.Empty);
            param.AddStringParam("logicAppName", logicAppName);
            param.AddStringParam("sqlConnectionName", request.DataStore.GetValue("sqlConnectionName"));
            param.AddStringParam("resourceGroup", resourceGroup);
            param.AddStringParam("subscription", subscription);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, logicAppJsonLocation)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            //Deploy logic app 
            var helper = new DeploymentHelper();
            var deploymentResponse = await helper.DeployLogicApp(subscription, azureToken, resourceGroup, armTemplate, deploymentName);

            if (!deploymentResponse.IsSuccess)
            {
                return deploymentResponse;
            }

            //Get logic app trigger Url
            AzureHttpClient azureClient = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var response = await azureClient.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"providers/Microsoft.Logic/workflows/{logicAppName}/triggers/manual/listCallbackUrl", "2016-06-01", string.Empty);

            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            var postUrl = JsonUtility.GetJObjectFromJsonString(await response.Content.ReadAsStringAsync());

            // Deploy logic app with updated trigger URL for last action
            var newParam = new AzureArmParameterGenerator();

            newParam.AddStringParam("logicAppTrigger", postUrl["value"].ToString());
            newParam.AddStringParam("logicAppName", logicAppName);
            newParam.AddStringParam("sqlConnectionName", request.DataStore.GetValue("sqlConnectionName"));
            newParam.AddStringParam("resourceGroup", resourceGroup);
            newParam.AddStringParam("subscription", subscription);

            armParamTemplate = JsonUtility.GetJObjectFromObject(newParam.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);
            
            deploymentResponse = await helper.DeployLogicApp(subscription, azureToken, resourceGroup, armTemplate, deploymentName);

            if (!deploymentResponse.IsSuccess)
            {
                return deploymentResponse;
            }
            
            request.DataStore.AddToDataStore("NotifierTriggerUrl", postUrl["value"].ToString());

            //Log logic app
            request.Logger.LogResource(request.DataStore, logicAppName,
                DeployedResourceType.LogicApp, CreatedBy.BPST, DateTime.UtcNow.ToString("o"));

            return new ActionResponse(ActionStatus.Success);
        }
    }
}