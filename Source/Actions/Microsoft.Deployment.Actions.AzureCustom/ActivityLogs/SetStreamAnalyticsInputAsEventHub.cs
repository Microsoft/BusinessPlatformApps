using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class SetStreamAnalyticsInputAsEventHub : BaseAction
    {
        // Configures Stream Analyitcs to receive data from Event Hub
        public async Task<bool> GetEventHubPrimaryKey(ActionRequest request, string token, string subscription, string resourceGroup, string ehnamespace)
        {
            string apiVersion = "2014-09-01";
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.EventHub/namespaces/{ehnamespace}/AuthorizationRules/RootManageSharedAccessKey/listkeys?api-version={apiVersion}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, uri, "{}");
            if (response.IsSuccessStatusCode)
            {
                string keyString = await response.Content.ReadAsStringAsync();
                JObject keys = JsonUtility.GetJObjectFromJsonString(keyString);
                string primaryKey = keys.GetValue("primaryKey").ToString();
                request.DataStore.AddToDataStore("primaryKey", primaryKey);
                return true;
            }
            return false;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string ehnamespace = request.DataStore.GetValue("ActivityLogNamespace");
            string apiVersion = "2015-10-01";
            string inputAlias = "EventHubInput";
            string jobName = request.DataStore.GetValue("SAJob");
            bool keyResponse = await GetEventHubPrimaryKey(request, token, subscription, resourceGroup, ehnamespace);
            if (!keyResponse) { return new ActionResponse(ActionStatus.Failure); }
            string primaryKey = request.DataStore.GetValue("primaryKey");
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/inputs/{inputAlias}?api-version={apiVersion}";
            string body = $"{{    \r\n   \"properties\":{{    \r\n      \"type\":\"stream\",  \r\n      \"serialization\":{{    \r\n         \"type\":\"Json\",  \r\n         \"properties\":{{    \r\n            \"fieldDelimiter\":\",\",  \r\n            \"encoding\":\"UTF8\"  \r\n         }}  \r\n      }},  \r\n      \"datasource\":{{    \r\n         \"type\":\"Microsoft.ServiceBus/EventHub\",  \r\n         \"properties\":{{    \r\n            \"serviceBusNamespace\":\"{ehnamespace}\",  \r\n            \"sharedAccessPolicyName\":\"RootManageSharedAccessKey\",  \r\n            \"sharedAccessPolicyKey\":\"{primaryKey}\",  \r\n            \"eventHubName\":\"insights-operational-logs\"  \r\n         }}  \r\n      }}  \r\n   }}  \r\n}}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            // API call to set entire input
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