using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Model.StreamAnalytics;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class GetHistoricalData : BaseAction
    {
        public static void BulkInsert(string connString, DataTable table, string tableName)
        {
            try
            {
                using (SqlBulkCopy bulk = new SqlBulkCopy(connString))
                {
                    bulk.BatchSize = 1000;
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer(table);
                    bulk.Close();
                }
            }
            catch
            {
                throw new Exception("overflow during batch insert in table " + tableName);
            }
        }

        public static DataTable createHistoricalDataTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("id");
            table.Columns.Add("caller");
            table.Columns.Add("correlationId");
            table.Columns.Add("description");
            table.Columns.Add("eventCategory");
            table.Columns.Add("impact");
            table.Columns.Add("impactedRegions");
            table.Columns.Add("jobFailedMessage");
            table.Columns.Add("level");
            table.Columns.Add("operationCategory");
            table.Columns.Add("operationId");
            table.Columns.Add("operationName");
            table.Columns.Add("resourceGroup");
            table.Columns.Add("resourceId");
            table.Columns.Add("status");
            table.Columns.Add("statusCode");
            table.Columns.Add("subscriptionId");
            table.Columns.Add("timestamp");
            return table;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            System.DateTime now = System.DateTime.UtcNow;
            System.DateTime days90ago = now.Subtract(new System.TimeSpan(2160, 0, 0));
            string nowString = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string days90agoString = days90ago.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            var geturi = $"https://management.azure.com/subscriptions/{subscription}/providers/microsoft.insights/eventtypes/management/values?api-version=2015-04-01&$filter=eventTimestamp ge '{days90agoString}' and eventTimestamp le '{nowString}' and eventChannels eq 'Admin, Operation'";
            // smaller time interval ~500 records below
            //var geturi = $"https://management.azure.com/subscriptions/{subscription}/providers/microsoft.insights/eventtypes/management/values?api-version=2015-04-01&$filter=eventTimestamp ge '2017-06-28T17:56:33.880Z' and eventTimestamp le '2017-06-29T17:56:33.880Z' and eventChannels eq 'Admin, Operation'";
            var sqlConn = request.DataStore.GetValue("SqlConnectionString");
            var historicalTable = createHistoricalDataTable();
            AzureHttpClient ahc = new AzureHttpClient(token, subscription);
            while (true)
            {
                ActivityLogResponse response = JsonUtility.Deserialize<ActivityLogResponse>(await ahc.Request(HttpMethod.Get, geturi));
                foreach (ActivityLogEntry activity in response.Value)
                {
                    DataRow historicalRow = historicalTable.NewRow();
                    if (activity.Claims != null)
                    {
                        historicalRow["caller"] = activity.Claims.Upn;
                    }
                    historicalRow["correlationId"] = activity.CorrelationId;
                    historicalRow["description"] = activity.Description;
                    if (activity.Category != null)
                    {
                        historicalRow["eventCategory"] = activity.Category.Value;
                    }
                    if (activity.Properties != null)
                    {
                        historicalRow["impact"] = activity.Properties.Impact;
                        historicalRow["impactedRegions"] = activity.Properties.ImpactedRegions;
                    }
                    historicalRow["level"] = activity.Level;
                    if (activity.OperationName != null)
                    {
                        string opName = activity.OperationName.Value;
                        if (opName.Length > 5)
                        {
                            int startidx;
                            if ((startidx = opName.LastIndexOf("/")) != -1)
                            {
                                string opCategory = opName.Substring(startidx + 1).ToLower();
                                if (opCategory == "write")
                                {
                                    historicalRow["operationCategory"] = "Write";
                                }
                                else if (opCategory == "delete")
                                {
                                    historicalRow["operationCategory"] = "Delete";
                                }
                                else if (opCategory == "action")
                                {
                                    historicalRow["operationCategory"] = "Action";
                                }
                            }
                        }
                    }
                    historicalRow["operationId"] = activity.OperationId;
                    if (activity.OperationName != null)
                    {
                        historicalRow["operationName"] = activity.OperationName.LocalizedValue;
                    }
                    historicalRow["resourceGroup"] = activity.ResourceGroupName;
                    historicalRow["resourceId"] = activity.Id;
                    if (activity.Status != null)
                    {
                        historicalRow["status"] = activity.Status.LocalizedValue;
                    }
                    if (activity.Properties != null)
                    {
                        historicalRow["statusCode"] = activity.Properties.StatusCode;
                    }
                    historicalRow["subscriptionId"] = activity.SubscriptionId;
                    historicalRow["timestamp"] = activity.EventTimestamp;
                    historicalTable.Rows.Add(historicalRow);
                }
                if ((geturi = response.NextLink) == null)
                {
                    break;
                }
            }
            BulkInsert(sqlConn, historicalTable, "aal.ActivityLogData");
            return new ActionResponse(ActionStatus.Success);
        }
    }
}