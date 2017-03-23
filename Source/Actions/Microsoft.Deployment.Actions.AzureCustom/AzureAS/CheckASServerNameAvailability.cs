using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class CheckASServerNameAvailability : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            string name = request.DataStore.GetValue("ASServerName");
            string location = request.DataStore.GetValue("ASLocation") ?? "westus";

            dynamic payload = new ExpandoObject();
            payload.name = name;
            payload.type = "Microsoft.AnalysisServices/servers";
            AzureHttpClient client = new AzureHttpClient(azureToken, subscription);
            HttpResponseMessage response = await client.ExecuteWithSubscriptionAsync(
                HttpMethod.Post, 
                $"providers/Microsoft.AnalysisServices/locations/{location}/checkNameAvailability", 
                "2016-05-16",
                JsonUtility.GetJsonStringFromObject(payload));

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                var json = JsonUtility.GetJsonObjectFromJsonString(body);
                if(json["nameAvailable"].ToString().ToLower() == "false")
                {
                    return new ActionResponse(ActionStatus.FailureExpected, json, null, null, json["reason"].ToString() + ": " + json["message"].ToString());
                }

                if (json["nameAvailable"].ToString().ToLower() == "true")
                {
                    return new ActionResponse(ActionStatus.Success, json);
                }
            }

            return new ActionResponse(ActionStatus.Failure,null,null,null,"Unable to query Azure for name availability");
        }
    }
}
