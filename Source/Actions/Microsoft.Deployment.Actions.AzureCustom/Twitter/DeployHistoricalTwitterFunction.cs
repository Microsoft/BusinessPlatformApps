using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

//NOT NEEDED
namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class DeployHistoricalTwitterFunction : BaseAction
    {
        public override async  Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            var deploymentName = request.DataStore.GetValue("DeploymentName");
            var sitename = request.DataStore.GetValue("SiteName");
            var functionAppHostingPlan = request.DataStore.GetValue("FunctionHostingPlan");

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("storageaccountname", "solutiontemplate" + Path.GetRandomFileName().Replace(".", "").Substring(0, 8));
            param.AddStringParam("sitename", sitename);
            param.AddStringParam("AppHostingPlan", functionAppHostingPlan);
            param.AddStringParam("resourcegroup", resourceGroup);
            param.AddStringParam("subscription", subscription);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/AzureArm/function.json")));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}