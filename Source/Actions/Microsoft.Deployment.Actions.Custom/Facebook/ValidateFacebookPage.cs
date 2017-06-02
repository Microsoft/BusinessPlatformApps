using System.ComponentModel.Composition;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Custom.Facebook
{
    [Export(typeof(IAction))]
    public class ValidateFacebookPage : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string clientId = request.DataStore.GetValue("FacebookClientId");
            string clientSecret = request.DataStore.GetValue("FacebookClientSecret");
            string page = request.DataStore.GetValue("FacebookPage");

            string requestUri = $"https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(requestUri);
            string responseObj = await response.Content.ReadAsStringAsync();
            if(!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.FailureExpected, responseObj);
            }

            string accessToken = JObject.Parse(responseObj)["access_token"].ToString();

            string pageRequestUri = $"https://graph.facebook.com/{page}/feed?access_token={accessToken}";
            response = await client.GetAsync(pageRequestUri);
            responseObj = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.FailureExpected, responseObj);
            }

            return new ActionResponse(ActionStatus.Success, responseObj);
        }
    }
}