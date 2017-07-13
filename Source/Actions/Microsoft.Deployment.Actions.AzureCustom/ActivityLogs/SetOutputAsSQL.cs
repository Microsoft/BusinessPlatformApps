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
    public class SetOutputAsSQL : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azure_access_token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var jobName = request.DataStore.GetValue("jobName");
            string apiVersion = "2015-10-01";
            var uri = $"https://managment.azure.com/subscriptions/{subscription}/resourceGroups/{resourceGroup}/providers/Microsoft.StreamAnalytics/streamingjobs/{jobName}/outputs/output?api-version={apiVersion}";
            var uri2 = $"https://main.streamanalytics.ext.azure.com/api/Outputs/PutOutput?fullResourceId=%2Fsubscriptions%2F{subscription}%2FresourceGroups%2F{resourceGroup}%2Fproviders%2FMicrosoft.StreamAnalytics%2Fstreamingjobs%2F{jobName}&subscriptionId={subscription}&resourceGroupName={resourceGroup}&jobName={jobName}&componentType=&componentName=";
            var server = request.DataStore.GetValue("serverName");
            var database = request.DataStore.GetValue("dbName");
            var user = request.DataStore.GetValue("username");
            var password = request.DataStore.GetValue("password");
            var table = request.DataStore.GetValue("tableName");
            var alias = request.DataStore.GetValue("outputAlias");
            var body = $"{{\"properties\":{{\"datasource\":{{\"type\":\"Microsoft.Sql/Server/Database\",\"properties\":{{\"server\":{server},\"database\":{database},\"table\":{table},\"user\":{user},\"password\":{password}}}}}}}}}";
            var body3 = $"{{\"properties\":{{\"dataSource\":{{\"outputDocumentDatabaseSource\":{{}},\"outputTopicSource\":{{}},\"outputQueueSource\":{{}},\"outputEventHubSource\":{{}},\"outputSqlDatabaseSource\":{{\"server\":{server},\"database\":{database},\"user\":{user},\"password\":{password},\"table\":{table}}},\"outputBlobStorageSource\":{{}},\"outputTableStorageSource\":{{}},\"outputPowerBISource\":{{}},\"outputDataLakeSource\":{{}},\"outputIotGatewaySource\":{{}},\"type\":\"Microsoft.Sql/Server/Database\"}},\"serialization\":{{}}}},\"createType\":\"None\",\"id\":null,\"location\":\"Australia East\",\"name\":{alias},\"type\":\"Microsoft.Sql/Server/Database\"}}";
            AzureHttpClient ahc = new AzureHttpClient(azure_access_token, subscription);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri2, body);
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);
        }
    }
}
