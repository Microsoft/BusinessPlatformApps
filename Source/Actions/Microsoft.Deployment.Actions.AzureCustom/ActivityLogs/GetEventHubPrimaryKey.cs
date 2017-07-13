using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class GetEventHubPrimaryKey : BaseAction
    {
        // Gets the primary policy key for the specified Event Hub namespace
        // Note/Question: A namespace can have multiple event hubs - is this/does this need to be accounted for?
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var ehnamespace = request.DataStore.GetValue("namespace");
            var apiVersion = "2014-09-01";
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.EventHub/namespaces/{ehnamespace}/AuthorizationRules/RootManageSharedAccessKey/listkeys?api-version={apiVersion}";

            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, uri, "{}");
            if (response.IsSuccessStatusCode)
            {
                string keyString = await response.Content.ReadAsStringAsync();
                JObject keys = JsonUtility.GetJObjectFromJsonString(keyString);
                string primaryKey = keys.GetValue("primaryKey").ToString();
                request.DataStore.AddToDataStore("primaryKey", primaryKey);
                return new ActionResponse(ActionStatus.Success);
            }
            return new ActionResponse(ActionStatus.Failure);
        }
    }
}
