using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Bpst;
using Microsoft.Deployment.Common.Model.EventHub;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class VerifyLogProfileEventHub : BaseAction
    {
        private const string INSIGHTS = "insights-operational-logs";
        private const int RETRIES = 1500;
        private TimeSpan SLEEP = new TimeSpan(0, 1, 0);

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            int attemptsInsights = request.DataStore.GetCount("attemptsInsights");
            string nameNamespace = request.DataStore.GetValue("nameNamespace");

            if (attemptsInsights < RETRIES)
            {
                attemptsInsights++;
                request.DataStore.AddToDataStore("attemptsInsights", attemptsInsights.ToString(), DataStoreType.Public);

                string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/resourceGroups/{ba.NameResourceGroup}/providers/Microsoft.EventHub/namespaces/{nameNamespace}/eventhubs?api-version=2015-08-01";
                string body = $"\"parameters\": {{\"namespaceName\":\"{nameNamespace}\",\"resourceGroupName\":\"{ba.NameResourceGroup}\",\"api-version\":\"2015-08-01\", \"subscriptionId\": \"{ba.IdSubscription}\"}}";

                AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure, ba.IdSubscription);

                List<EventHub> hubs = await ahc.RequestValue<List<EventHub>>(HttpMethod.Get, url, body);

                if (!hubs.IsNullOrEmpty())
                {
                    foreach (EventHub hub in hubs)
                    {
                        if (hub.Name.EqualsIgnoreCase(INSIGHTS))
                        {
                            return new ActionResponse(ActionStatus.Success);
                        }
                    }
                }

                Thread.Sleep(SLEEP);

                return new ActionResponse(ActionStatus.InProgress);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("ActivityLogsVerifyInsightsTimeout"));
            }

        }
    }
}