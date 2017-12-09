using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class SetAdministrativeOutput : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azure_access_token = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string jobName = request.DataStore.GetValue("SAJob");
            string apiVersion = "2015-10-01";
            string outputAlias = "AdministrativeOutput";
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/outputs/{outputAlias}?api-version={apiVersion}";
            string server = request.DataStore.GetValue("Server");
            string database = request.DataStore.GetValue("Database");
            string user = request.DataStore.GetValue("Username");
            string password = request.DataStore.GetValue("Password");
            string table = request.DataStore.GetValue("AdministrativeTable");
            string body = $"{{\"properties\":{{\"datasource\":{{\"type\":\"Microsoft.Sql/Server/Database\",\"properties\":{{\"server\":\"{server}\",\"database\":\"{database}\",\"table\":\"{table}\",\"user\":\"{user}\",\"password\":\"{password}\"}}}}}}}}";
            AzureHttpClient ahc = new AzureHttpClient(azure_access_token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri, body);
            if (!response.IsSuccessStatusCode)
            {
                for (int i = 0; i < 5; i++)
                {
                    response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri, body);
                    if (response.IsSuccessStatusCode)
                    {
                        return new ActionResponse(ActionStatus.Success);
                    }
                    Thread.Sleep(4000);
                }
            }
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}