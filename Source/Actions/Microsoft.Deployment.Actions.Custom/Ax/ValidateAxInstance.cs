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

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class ValidateAxInstance : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var axToken = request.DataStore.GetJson("AxToken", "access_token").ToString();
            string instance = request.DataStore.GetValue("AxInstanceName");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(instance);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", axToken.ToString());

                var response = await client.GetAsync($"data/BpstConfigurationEntities");

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), null, "AxWrongPlatform", response.Content.ReadAsStringAsync().Result);
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
