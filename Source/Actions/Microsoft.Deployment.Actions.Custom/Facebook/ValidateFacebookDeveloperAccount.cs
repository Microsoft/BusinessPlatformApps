using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using System.Net.Http;

namespace Microsoft.Deployment.Actions.Custom.Facebook
{
    [Export(typeof(IAction))]
    public class ValidateFacebookDeveloperAccount : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string clientId = request.DataStore.GetValue("FacebookClientId");
            string clientSecret = request.DataStore.GetValue("FacebookClientSecret");

            string requestUri = $"https://graph.facebook.com/oauth/access_token?grant_type=client_credentials&client_id={clientId}&client_secret={clientSecret}";

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                response = await client.GetAsync(requestUri);
            }
            
            string responseObj = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.FailureExpected, responseObj);
            }

            return new ActionResponse(ActionStatus.Success, responseObj);
        }
    }
}