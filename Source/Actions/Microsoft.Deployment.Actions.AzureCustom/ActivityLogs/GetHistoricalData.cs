using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Model.StreamAnalytics;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class GetHistoricalData : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string sqlConn = request.DataStore.GetValue("SqlConnectionString");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string token = request.DataStore.GetJson("AzureToken", "access_token");

            System.DateTime now = System.DateTime.UtcNow;

            System.DateTime days90ago = now.Subtract(new System.TimeSpan(2160, 0, 0));

            string days90agoString = days90ago.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            string nowString = now.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

            string geturi = string.IsNullOrEmpty(request.DataStore.GetValue("NextLink"))
                ? $"https://management.azure.com/subscriptions/{subscription}/providers/microsoft.insights/eventtypes/management/values?api-version=2015-04-01&$filter=eventTimestamp ge '{days90agoString}' and eventTimestamp le '{nowString}' and eventChannels eq 'Admin, Operation'"
                : request.DataStore.GetValue("NextLink");

            DataTable shTable = CreateServiceHealthTable();
            DataTable adminTable = CreateAdministrativeTable();

            AzureHttpClient ahc = new AzureHttpClient(token, subscription);

            ActivityLogResponse response = await ahc.Request<ActivityLogResponse>(HttpMethod.Get, geturi);

            if (response == null || response.Value.IsNullOrEmpty())
            {
                //return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("ActivityLogsGetHistoricalDataError"));
                // Historical Data is optional - let users continue even if not all their historical data is imported
                return new ActionResponse(ActionStatus.Success);
            }

            foreach (ActivityLogEntry activity in response.Value)
            {
                if (activity.Category != null)
                {
                    if (activity.Category.Value == "ServiceHealth")
                    {
                        DataRow shRow = shTable.NewRow();

                        shRow["serviceHealthId"] = activity.EventDataId;
                        shRow["correlationId"] = activity.CorrelationId;
                        shRow["description"] = activity.Description;

                        if (activity.Properties != null)
                        {
                            shRow["impact"] = activity.Properties.Impact;
                            shRow["impactedRegions"] = activity.Properties.ImpactedRegions;
                            shRow["impactedServices"] = activity.Properties.ImpactedServices;
                            shRow["incidentType"] = activity.Properties.IncidentType;
                        }

                        shRow["level"] = activity.Level;
                        shRow["operationId"] = activity.OperationId;

                        if (activity.Status != null)
                        {
                            shRow["status"] = activity.Status.LocalizedValue;
                        }

                        shRow["timestamp"] = activity.EventTimestamp;

                        if (activity.Properties != null)
                        {
                            shRow["title"] = activity.Properties.Title;
                        }

                        shTable.Rows.Add(shRow);
                    }
                }

                DataRow adminRow = adminTable.NewRow();

                if (activity.Claims != null)
                {
                    adminRow["caller"] = activity.Claims.Upn;
                }

                adminRow["correlationId"] = activity.CorrelationId;
                adminRow["description"] = activity.Description;

                if (activity.Category != null)
                {
                    adminRow["eventCategory"] = activity.Category.Value;
                }

                adminRow["level"] = activity.Level;

                if (activity.OperationName != null)
                {
                    string opName = activity.OperationName.Value;

                    if (!string.IsNullOrEmpty(opName) && opName.Length > 5)
                    {
                        int startidx;

                        if ((startidx = opName.LastIndexOf("/")) != -1)
                        {
                            string opCategory = opName.Substring(startidx + 1);

                            if (opCategory.EqualsIgnoreCase("write"))
                            {
                                adminRow["operationCategory"] = "Write";
                            }
                            else if (opCategory.EqualsIgnoreCase("delete"))
                            {
                                adminRow["operationCategory"] = "Delete";
                            }
                            else if (opCategory.EqualsIgnoreCase("action"))
                            {
                                adminRow["operationCategory"] = "Action";
                            }
                        }
                    }
                }

                adminRow["operationId"] = activity.OperationId;

                if (activity.OperationName != null)
                {
                    adminRow["operationName"] = activity.OperationName.LocalizedValue;
                }

                adminRow["resourceGroup"] = activity.ResourceGroupName;
                adminRow["resourceId"] = activity.Id;

                if (activity.ResourceProviderName != null && activity.ResourceProviderName.Value != null)
                {
                    adminRow["resourceProvider"] = activity.ResourceProviderName.Value.ToUpper();
                }

                if (activity.Status != null)
                {
                    adminRow["status"] = activity.Status.LocalizedValue;
                }

                adminRow["timestamp"] = activity.EventTimestamp;

                adminTable.Rows.Add(adminRow);
            }

            BulkInsert(sqlConn, shTable, "bpst_aal.ServiceHealthData");
            BulkInsert(sqlConn, adminTable, "bpst_aal.AdministrativeData");

            request.DataStore.AddToDataStore("NextLink", response.NextLink, DataStoreType.Public);

            return response.NextLink == null
                ? new ActionResponse(ActionStatus.Success)
                : new ActionResponse(ActionStatus.InProgress);
        }

        private static void BulkInsert(string connString, DataTable table, string tableName)
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

        private static DataTable CreateAdministrativeTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("eventId");
            table.Columns.Add("caller");
            table.Columns.Add("correlationId");
            table.Columns.Add("description");
            table.Columns.Add("eventCategory");
            table.Columns.Add("level");
            table.Columns.Add("operationCategory");
            table.Columns.Add("operationId");
            table.Columns.Add("operationName");
            table.Columns.Add("resourceGroup");
            table.Columns.Add("resourceId");
            table.Columns.Add("resourceProvider");
            table.Columns.Add("status");
            table.Columns.Add("timestamp");
            return table;
        }

        private static DataTable CreateServiceHealthTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("serviceHealthId");
            table.Columns.Add("correlationId");
            table.Columns.Add("description");
            table.Columns.Add("impact");
            table.Columns.Add("impactedRegions");
            table.Columns.Add("impactedServices");
            table.Columns.Add("incidentType");
            table.Columns.Add("level");
            table.Columns.Add("operationId");
            table.Columns.Add("status");
            table.Columns.Add("timestamp");
            table.Columns.Add("title");
            return table;
        }
    }
}