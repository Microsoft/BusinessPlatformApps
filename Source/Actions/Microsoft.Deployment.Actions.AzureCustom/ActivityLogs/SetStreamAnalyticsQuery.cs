using System.ComponentModel.Composition;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class SetStreamAnalyticsQuery : BaseAction
    {
        // Updates the default query provided by a Stream Analytics job
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string jobName = request.DataStore.GetValue("nameStreamAnalyticsJob");
            string transformationName = "Transformation";
            string apiVersion = "2015-10-01";
            string query = File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/TextHelpers/SAquery.txt"));
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/transformations/{transformationName}?api-version={apiVersion}";
            string input = request.DataStore.GetValue("inputAlias");
            string output = request.DataStore.GetValue("outputAlias");
            string body = $"{{\"properties\":{{\"streamingUnits\":1,\"query\":\"{query}\"}}}}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri, body);
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}