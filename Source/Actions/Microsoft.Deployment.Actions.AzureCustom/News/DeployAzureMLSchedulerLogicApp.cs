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

namespace Microsoft.Deployment.Actions.AzureCustom.Newws
{
    [Export(typeof(IAction))]
    public class DeployAzureMLSchedulerLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var storageAccountName = request.DataStore.GetValue("StorageAccountName");
            var logicAppName = request.DataStore.GetValue("LogicAppName");

            var apiKeyTopics = request.DataStore.GetAllValues("AzureMLKey")[0];
            var apiKeyImages = request.DataStore.GetAllValues("AzureMLKey")[1]; ;
            var apiKeyEntities = request.DataStore.GetAllValues("AzureMLKey")[2]; ;

            var apiUrlTopics = request.DataStore.GetAllValues("AzureMLUrl")[0];
            var apiUrlImages = request.DataStore.GetAllValues("AzureMLUrl")[1];
            var apiUrlEntities = request.DataStore.GetAllValues("AzureMLUrl")[2];

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            //param.AddStringParam("storageaccountname", storageAccountName);
            param.AddStringParam("logicappname", logicAppName);
            param.AddStringParam("apikeytopics", apiKeyTopics);
            param.AddStringParam("apikeyimages", apiKeyImages);
            param.AddStringParam("apikeyentities", apiKeyEntities);
            param.AddStringParam("apiurltopics", apiUrlTopics);
            param.AddStringParam("apiurlentities", apiUrlEntities);
            param.AddStringParam("apiurlimages", apiUrlImages);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/AzureArm/AzureMLSchedulerLogicApp.json")));
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
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null,
                     DefaultErrorCodes.DefaultErrorCode, $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = await client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());
            return new ActionResponse(ActionStatus.Success, deploymentItem);
        }
    }
}
