﻿using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class SetStreamAnalyticsQuery : BaseAction
    {
        // Updates the default query provided by a Stream Analytics job
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var jobName = request.DataStore.GetValue("SAJob");
            var transformationName = "Transformation";
            var apiVersion = "2015-10-01";
            string query = File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/TextHelpers/SAquery.txt"));
            string uri = $"https://management.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/transformations/{transformationName}?api-version={apiVersion}";
            string input = request.DataStore.GetValue("inputAlias");
            string output = request.DataStore.GetValue("outputAlias");
            var body = $"{{\"properties\":{{\"streamingUnits\":1,\"query\":\"{query}\"}}}}";
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri, body);
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}