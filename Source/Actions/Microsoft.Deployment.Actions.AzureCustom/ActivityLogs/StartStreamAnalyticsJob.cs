using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class StartStreamAnalyticsJob : BaseAction
    {
        // Starts the specified Stream Analytics job
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string apiVersion = "2015-10-01";
            string jobName = request.DataStore.GetValue("SAJob");
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/start?api-version={apiVersion}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, uri, "{}");
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);

        }
    }
}