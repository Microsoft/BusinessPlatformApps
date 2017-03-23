using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Actions.Salesforce.Models;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Actions.Salesforce.Helpers;
using System.Globalization;
using System.Data;
using System.Net.Http;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    class ADFSliceStatus : BaseAction
    {
        private string apiVersion = "2015-10-01";
        private string getDatasetRelativeUrl = "providers/Microsoft.DataFactory/datafactories/{0}/datasets";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var dataFactory = resourceGroup + "SalesforceCopyFactory";

            var url = string.Format(getDatasetRelativeUrl, dataFactory);


            DataTable table = new DataTable();
            table.Columns.Add("Dataset");
            table.Columns.Add("Start");
            table.Columns.Add("End");
            table.Columns.Add("Status");

            var client = new AzureHttpClient(token, subscription, resourceGroup);

            var connection = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get,
                  url, apiVersion, string.Empty);

            if (!connection.IsSuccessStatusCode)
            {
                var result = connection.Content.ReadAsStringAsync().Result;
                return new ActionResponse(ActionStatus.FailureExpected,
                    JsonUtility.GetJObjectFromJsonString(result), null, DefaultErrorCodes.DefaultErrorCode,
                    result);
            }

            var connectionData = JsonUtility.GetJObjectFromJsonString(connection.Content.ReadAsStringAsync().Result);

            if (connectionData != null)
            {
                foreach (var dataset in connectionData["value"])
                {
                    var nameParts = dataset["name"].ToString().Split('_');
                    if (nameParts[0] == "PreDeployment" && nameParts[2] == "Output")
                    {
                        Dictionary<string, string> queryParameters = new Dictionary<string, string>();
                        queryParameters.Add("start", DateTime.UtcNow.AddYears(-3).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
                        queryParameters.Add("end", DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
                        var sliceRelativeUrl = string.Concat(url, '/', dataset["name"].ToString());
                        sliceRelativeUrl.Remove(0, 1);
                        var sliceConnection = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get,
                            sliceRelativeUrl + "/slices/",
                            apiVersion,
                            string.Empty,
                            queryParameters
                            );

                        if (!sliceConnection.IsSuccessStatusCode)
                        {
                            var result = connection.Content.ReadAsStringAsync().Result;
                            return new ActionResponse(ActionStatus.FailureExpected,
                                JsonUtility.GetJObjectFromJsonString(result), null, DefaultErrorCodes.DefaultErrorCode,
                                result);
                        }

                        var data = JsonUtility.GetJObjectFromJsonString(sliceConnection.Content.ReadAsStringAsync().Result);

                        if (data["value"].Count() > 2)
                        {
                            int numberOfSlices = data["value"].Count() - 2;

                            var lastSlice = data["value"][numberOfSlices];

                            table.Rows.Add(new[] { dataset["name"].ToString().Split('_')[1], lastSlice["start"].ToString(), lastSlice["end"].ToString(), lastSlice["status"].ToString() });
                        }
                        else
                        {
                            table.Rows.Add(new[] { dataset["name"].ToString().Split('_')[1], string.Empty, string.Empty, data["value"][0]["status"].ToString() });
                        }
                    }
                }
            }

            var ready = from DataRow row in table.Rows
                        where (string)row["Status"] != "Ready"
                        select (string)row["Dataset"];

            var response = JsonUtility.CreateJObjectWithValueFromObject(table);

            if (ready.Count() > 0)
            {
                return new ActionResponse(ActionStatus.BatchNoState, response);
            }
            else
            {
                return new ActionResponse(ActionStatus.Success, response);
            }
        }
    }
}
