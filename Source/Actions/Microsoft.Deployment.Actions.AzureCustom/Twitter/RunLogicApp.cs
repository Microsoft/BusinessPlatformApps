using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class RunLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var logicAppName = request.DataStore.GetValue("LogicAppName");
            var requestUri = request.DataStore.GetValue("RequestUri");


            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            string body = "{\"PreviousCount\": 1, \"Delay\": 9}";

            var response = await client.ExecuteGenericRequestNoHeaderAsync(HttpMethod.Post, requestUri, body);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}