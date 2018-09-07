using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Deployment.Common.Helpers;
using System.Net;
using Newtonsoft.Json;
using System.Dynamic;
using Microsoft.Deployment.Common;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class TriggerCDSAIntegrationProcess : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var axToken = request.DataStore.GetJson("AxToken", "access_token").ToString();
            string axInstance = request.DataStore.GetValue("AxInstanceName");
            string axInstanceAction = request.DataStore.GetValue("AxInstanceEntityAction");
            string selectedMeasurements = request.DataStore.GetValue("SelectedMeasurements");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(axInstance);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", axToken.ToString());

                dynamic payload = new ExpandoObject();
                payload.measurementName = selectedMeasurements;

                var response = await client.PostAsync(axInstance.Trim() + axInstanceAction, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    return new ActionResponse(ActionStatus.Success);
                }

                switch (response.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), null, "AxWrongPlatform", response.Content.ReadAsStringAsync().Result);
                    case HttpStatusCode.Unauthorized:
                        return new ActionResponse(ActionStatus.Failure, response.ReasonPhrase, null, "AxAuthorizationError", response.ReasonPhrase);
                    default:
                        return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), null, "AxConnectionError", response.Content.ReadAsStringAsync().Result);
                }
            }
        }
    }
}
