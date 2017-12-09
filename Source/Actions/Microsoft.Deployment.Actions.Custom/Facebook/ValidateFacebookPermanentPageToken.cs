using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Custom.Facebook
{
    [Export(typeof(IAction))]
    public class ValidateFacebookPermanentPageToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string page = request.DataStore.GetValue("PageId");
            string permanentPageToken = request.DataStore.GetValue("PermanentPageToken");

            var actionResponse = new ActionResponse();

            string pageRequestUri = $"https://graph.facebook.com/v2.10/{page}/insights?access_token={permanentPageToken}&metric=page_impressions";
            HttpResponseMessage response;
            string responseObj;

            using (HttpClient client = new HttpClient())
            {
                response = await client.GetAsync(pageRequestUri);
                responseObj = await response.Content.ReadAsStringAsync();

                var data = JsonUtility.GetJObjectFromJsonString(responseObj)["data"];

                if (data != null && data.Children().Count() > 0)

                    actionResponse = new ActionResponse(ActionStatus.Success);
            }

            if (!response.IsSuccessStatusCode)
            {
                actionResponse = new ActionResponse(ActionStatus.FailureExpected, null, null, $"FacebookTokenAuthFailed", JsonUtility.GetJObjectFromJsonString(responseObj)["error"]["message"].ToString());
            }

            return actionResponse;
        }
    }
}
