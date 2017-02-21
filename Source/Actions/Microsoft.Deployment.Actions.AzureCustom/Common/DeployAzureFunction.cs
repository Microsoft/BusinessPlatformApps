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

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployAzureFunction : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var repoUrl = request.DataStore.GetValue("RepoUrl");
            var name = request.DataStore.GetValue("FunctionName");

            var hostingPlanName = request.DataStore.GetValue("hostingPlanName") == null ? "apphostingplan" : request.DataStore.GetValue("hostingPlanName");
            var hostingEnvironment = request.DataStore.GetValue("hostingEnvironment") == null ? "": request.DataStore.GetValue("hostingEnvironment");
            var sku = request.DataStore.GetValue("sku") == null ? "Dynamic" : request.DataStore.GetValue("sku");
            var skuCode = request.DataStore.GetValue("skuCode") == null ? "S1" : request.DataStore.GetValue("skuCode");
            var workerSize = request.DataStore.GetValue("workerSize") == null ? "0" : request.DataStore.GetValue("workerSize");

            string functionArmDeploymentRelatovePath = sku.ToLower() == "standard"
                ? "Service/Arm/AzureFunctionsStaticAppPlan.json"
                : "Service/Arm/AzureFunctions.json";

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("storageaccountname", "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8));
            param.AddStringParam("name", name);
            param.AddStringParam("repoUrl", repoUrl);
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);
            param.AddStringParam("hostingPlanName", hostingPlanName);
            param.AddStringParam("hostingEnvironment", hostingEnvironment);
            param.AddStringParam("sku", sku);
            param.AddStringParam("skuCode", skuCode);
            param.AddStringParam("workerSize", workerSize);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, functionArmDeploymentRelatovePath)));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);


            var deployment = new Azure.Management.Resources.Models.Deployment()
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
