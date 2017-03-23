using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class GetTwitterConnection : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");

            HttpResponseMessage connection = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get,
                "/providers/Microsoft.Web/connections/twitter", "2015-08-01-preview", string.Empty);

            if (!connection.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()), null, 
                    DefaultErrorCodes.DefaultErrorCode, "Failed to get consent");
            }

            return new ActionResponse(ActionStatus.Success, await connection.Content.ReadAsStringAsync());
        }
    }
}