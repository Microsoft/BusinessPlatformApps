using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class GetLogicAppUrl : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var logicAppName = request.DataStore.GetValue("LogicAppName");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"providers/Microsoft.Logic/workflows/{logicAppName}/triggers/manual/listCallbackUrl", "2016-06-01", string.Empty);

            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            var postUrl = JsonUtility.GetJObjectFromJsonString(await response.Content.ReadAsStringAsync());
            request.DataStore.AddToDataStore("RequestUri", postUrl["value"].ToString());

            return new ActionResponse(ActionStatus.Success);
        }
    }
}