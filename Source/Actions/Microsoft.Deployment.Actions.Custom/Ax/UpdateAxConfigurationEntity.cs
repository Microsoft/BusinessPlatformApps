using System;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class UpdateAxConfigurationEntity : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var axToken = request.DataStore.GetJson("AxToken", "access_token").ToString();
            string connString = request.DataStore.GetValue("SqlConnectionString");
            string measurementName = request.DataStore.GetValue("AxMeasurementName");
            string reportName = request.DataStore.GetValue("AxReportName");
            string instance = request.DataStore.GetValue("AxInstanceName");

            string id = string.Empty;
            string dataAreaId = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(instance);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", axToken.ToString());

                var existingEntities = await client.GetAsync($"data/BpstConfigurationEntities?$filter=ReportName eq '{reportName}' and MeasurementName eq '{measurementName}'");

                if (!existingEntities.IsSuccessStatusCode)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(existingEntities.Content.ReadAsStringAsync().Result), null, null, existingEntities.Content.ReadAsStringAsync().Result);
                }

                var entitiesObject = JsonUtility.GetJObjectFromJsonString(existingEntities.Content.ReadAsStringAsync().Result);
                HttpResponseMessage update = new HttpResponseMessage();

                if (entitiesObject["value"].Count() != 0)
                {
                    var firstEntry = entitiesObject["value"][0];

                    dynamic updatePayload = new ExpandoObject();
                    updatePayload.Id = firstEntry["Id"];
                    updatePayload.dataAreaId = firstEntry["dataAreaId"];
                    updatePayload.MeasurementName = measurementName;
                    updatePayload.ReportName = reportName;
                    updatePayload.SqlConnectionString = connString;

                    var msg = new HttpRequestMessage();
                    msg.Method = new HttpMethod("MERGE");
                    msg.Headers.Add("If-Match", firstEntry["@odata.etag"].ToString());
                    msg.RequestUri = new Uri(instance.Trim() + $"data/BpstConfigurationEntities?$filter=ReportName eq '{reportName}' and dataAreaId eq {firstEntry["dataAreaId"]} and Id eq {firstEntry["Id"]}  and MeasurementName eq '{measurementName}'");
                    msg.Content = new StringContent(JsonConvert.SerializeObject(updatePayload), Encoding.UTF8, "application/json");
                    update = await client.SendAsync(msg);
                    id = firstEntry["Id"].ToString();
                    dataAreaId = firstEntry["dataAreaId"].ToString();
                }

                if (entitiesObject["value"].Count() == 0 || !update.IsSuccessStatusCode)
                {
                    dynamic payload = new ExpandoObject();
                    payload.MeasurementName = measurementName;
                    payload.ReportName = reportName;
                    payload.SqlConnectionString = connString;

                    var resp = await client.PostAsync($"data/BpstConfigurationEntities", new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                    var entry = JsonUtility.GetJObjectFromJsonString(resp.Content.ReadAsStringAsync().Result);

                    id = entry["Id"]?.ToString();
                    dataAreaId = entry["dataAreaId"]?.ToString();

                    if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        return new ActionResponse(ActionStatus.Failure, null, null, null, "Token expired or user not authorized. Error:" + resp.ReasonPhrase + "," + resp.Content.ReadAsStringAsync().Result);
                    }
                }
            }

            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(dataAreaId))
            {
                return new ActionResponse(ActionStatus.Failure, "Entity was not created successfully.");
            }

            request.DataStore.AddToDataStore("AxEntityId", id, DataStoreType.Private);
            request.DataStore.AddToDataStore("AxEntityDataAreaId", dataAreaId, DataStoreType.Private);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
