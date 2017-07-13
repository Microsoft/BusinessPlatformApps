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
    public class SetStreamAnalyticsInputAsEventHub : BaseAction
    {
        // Configures Stream Analyitcs to receive data from Event Hub
        public async Task<bool> VerifyInputAlias(string alias, string subscription, string resourceGroup, string jobName, AzureHttpClient ahc)
        {
            // API call to verify the availability of the provided input alias
            var uri = $"https://main.streamanalytics.ext.azure.com/api/Jobs/IsEndpointNameAvailable?subscriptionId={subscription}&resourceGroupName={resourceGroup}&jobName={jobName}&endpointName={alias}";
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, uri, "{}");
            string responseString =  await response.Content.ReadAsStringAsync();
            JObject responseObj = JsonUtility.GetJObjectFromStringValue(responseString);
            return (responseObj.GetValue("value").ToString() == "true") ? true : false;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var ehnamespace = request.DataStore.GetValue("namespace");
            var apiVersion = "2015-10-01";
            var alias = request.DataStore.GetValue("inputAlias");
            var jobName = request.DataStore.GetValue("jobName");
            string primaryKey = request.DataStore.GetValue("primaryKey");
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/inputs/{alias}?api-version={apiVersion}";
            var body = $"{{    \r\n   \"properties\":{{    \r\n      \"type\":\"stream\",  \r\n      \"serialization\":{{    \r\n         \"type\":\"Json\",  \r\n         \"properties\":{{    \r\n            \"fieldDelimiter\":\",\",  \r\n            \"encoding\":\"UTF8\"  \r\n         }}  \r\n      }},  \r\n      \"datasource\":{{    \r\n         \"type\":\"Microsoft.ServiceBus/EventHub\",  \r\n         \"properties\":{{    \r\n            \"serviceBusNamespace\":\"{ehnamespace}\",  \r\n            \"sharedAccessPolicyName\":\"RootManageSharedAccessKey\",  \r\n            \"sharedAccessPolicyKey\":\"{primaryKey}\",  \r\n            \"eventHubName\":\"insights-operational-logs\"  \r\n         }}  \r\n      }}  \r\n   }}  \r\n}}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            bool isValidAlias = await VerifyInputAlias(alias, subscription, resourceGroup, jobName, ahc);
            if (!isValidAlias) return new ActionResponse(ActionStatus.Failure);
            // API call to set entire input
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri, body);
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}
