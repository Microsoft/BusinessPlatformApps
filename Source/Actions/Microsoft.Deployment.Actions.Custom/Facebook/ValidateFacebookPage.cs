using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

namespace Microsoft.Deployment.Actions.Custom.Facebook
{
    [Export(typeof(IAction))]
    public class ValidateFacebookPage : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string clientId = request.DataStore.GetValue("FacebookClientId");
            string clientSecret = request.DataStore.GetValue("FacebookClientSecret");
            string pages = request.DataStore.GetValue("FacebookPages");

            foreach (var pageToSearch in pages.Split(','))
            {
                string page = pageToSearch.Replace(" ", "");
                string requestUri = $"https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

                HttpResponseMessage response;
                string responseObj;

                using (HttpClient client = new HttpClient())
                {
                    response = await client.GetAsync(requestUri);
                    responseObj = await response.Content.ReadAsStringAsync();
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ActionResponse(ActionStatus.FailureExpected, responseObj);
                    }

                    string accessToken = JObject.Parse(responseObj)["access_token"].ToString();

                    string pageRequestUri = $"https://graph.facebook.com/{page}?access_token={accessToken}&fields=picture,name";
                    response = await client.GetAsync(pageRequestUri);
                }

                responseObj = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    return new ActionResponse(ActionStatus.FailureExpected, null, null, $"FacebookPagesInvalid", $"{page} not found");
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}