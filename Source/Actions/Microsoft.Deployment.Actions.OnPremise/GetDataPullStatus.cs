using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.OnPremise
{
    [Export(typeof(IAction))]
    public class GetDataPullStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            ActionResponse response;

            bool isWaitingForAtLeastOneRecord = request.DataStore.GetValue("IsWaiting") == null
                ? false
                : bool.Parse(request.DataStore.GetValue("IsWaiting"));

            string connectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex"); // Must specify Initial Catalog
            string finishedActionName = request.DataStore.GetValue("FinishedActionName");
            string targetSchema = request.DataStore.GetValue("TargetSchema"); // Specifies the schema used by the template

            string query = $"[{targetSchema}].sp_get_replication_counts";

            DataTable recordCounts;

            try
            {
                recordCounts = SqlUtility.InvokeStoredProcedure(connectionString, query, null);
            }
            catch
            {
                // It's ok for this to fail, we'll just return an empty table
                recordCounts = new DataTable();
            }

            bool isAtLeastOneRecordComingIn = false;

            if (isWaitingForAtLeastOneRecord)
            {
                foreach (DataRow row in recordCounts.Rows)
                {
                    isAtLeastOneRecordComingIn = Convert.ToInt64(row["Count"]) > 0;
                    if (isAtLeastOneRecordComingIn)
                        break;
                }

                response = isAtLeastOneRecordComingIn
                    ? new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject())
                    : new ActionResponse(ActionStatus.BatchNoState, JsonUtility.GetEmptyJObject());
            }
            else
            {
                response = new ActionResponse(ActionStatus.Success, JsonUtility.GetJsonObjectFromJsonString("{isFinished:false,status:" + JsonUtility.Serialize(recordCounts) + "}"));
            }

            if (string.IsNullOrEmpty(finishedActionName))
                return response;

            ActionResponse finishedResponse = await RequestUtility.CallAction(request, finishedActionName);

            if (response.Status == ActionStatus.BatchNoState && finishedResponse.Status == ActionStatus.BatchNoState)
                return response;

            var content = JObject.FromObject(finishedResponse.Body)["value"]?.ToString();
            if ((isAtLeastOneRecordComingIn && finishedResponse.Status != ActionStatus.Failure) || finishedResponse.Status == ActionStatus.Success)
            {
                var resp = new ActionResponse();
                if (!string.IsNullOrEmpty(content))
                {
                    resp = new ActionResponse(ActionStatus.Success,
                        JsonUtility.GetJsonObjectFromJsonString(
                        "{isFinished:true,FinishedActionName:\"" +
                        finishedActionName +
                        "\",TargetSchema:\"" + targetSchema +
                        "\",status:" + JsonUtility.Serialize(recordCounts) +
                        ", slices:" + JObject.FromObject(finishedResponse.Body)["value"]?.ToString() + "}"));
                }
                else
                {
                    resp = new ActionResponse(ActionStatus.Success,
                        JsonUtility.GetJsonObjectFromJsonString(
                            "{isFinished:true, status:" + JsonUtility.Serialize(recordCounts) + "}"));
                }
                return resp;
            }

            if (finishedResponse.Status == ActionStatus.BatchNoState || finishedResponse.Status == ActionStatus.BatchWithState)
            {
                var resp = new ActionResponse();
                if (!string.IsNullOrEmpty(content))
                {
                    resp = new ActionResponse(ActionStatus.Success,
                        JsonUtility.GetJsonObjectFromJsonString(
                    "{isFinished:false,FinishedActionName:\"" +
                    finishedActionName +
                     "\",TargetSchema:\"" + targetSchema +
                     "\",status:" + JsonUtility.Serialize(recordCounts) +
                    ", slices:" + JObject.FromObject(finishedResponse.Body)["value"]?.ToString() + "}"));
                }
                else
                {
                    resp = new ActionResponse(ActionStatus.Success,
                        JsonUtility.GetJsonObjectFromJsonString(
                        "{isFinished:false, status:" + JsonUtility.Serialize(recordCounts) + "}"));
                }
                return resp;
            }

            return finishedResponse;
        }
    }
}