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
    public class CreateAzureStorageAccount : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var name = request.DataStore.GetValue("StorageAccountName");
            var accountType = request.DataStore.GetValue("StorageAccountType");
            var encryptionEnabled = request.DataStore.GetValue("StorageAccountEncryptionEnabled");



            var param = new AzureArmParameterGenerator();
            param.AddStringParam("storageaccountname", "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8));
            param.AddStringParam("encryptionEnabled", encryptionEnabled);
            param.AddStringParam("accountType", accountType);
            param.AddStringParam("location", location);
            param.AddStringParam("name", name);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, "Service/Arm/storageAccount.json")));
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
