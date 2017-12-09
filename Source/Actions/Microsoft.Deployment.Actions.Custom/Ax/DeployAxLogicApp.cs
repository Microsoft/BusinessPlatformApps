using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using System.ComponentModel.Composition;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    class DeployAxLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string deploymentName = request.DataStore.GetValue("DeploymentName");
            string logicAppTrigger = string.Empty;
            string areaId = request.DataStore.GetValue("AxEntityDataAreaId");
            string id = request.DataStore.GetValue("AxEntityId");
            string connectionName = request.DataStore.GetValue("connectorName");
            string instanceUrl = request.DataStore.GetValue("AxInstanceName").Replace("https://", string.Empty).Replace("/", string.Empty);

            // Read from file
            var logicAppJsonLocation = "Service/LogicApp/axLogicApp.json";

            if (deploymentName == null)
            {
                deploymentName = request.DataStore.CurrentRoute;
            }

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourceGroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("connectionName", connectionName);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, logicAppJsonLocation)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);
            var armString = armTemplate.ToString().Replace("ENTITYID", id).Replace("ENTITYAREAID", areaId).Replace("AXINSTANCEURL", instanceUrl);

            //Deploy logic app 
            var helper = new DeploymentHelper();
            var deploymentResponse = await helper.DeployLogicApp(subscription, azureToken, resourceGroup, JsonUtility.GetJObjectFromJsonString(armString), deploymentName);

            if (!deploymentResponse.IsSuccess)
            {
                return deploymentResponse;
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}