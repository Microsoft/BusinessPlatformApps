using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class ValidateCognitiveKey : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            //Request headers
            var subscriptionKey = request.DataStore.GetValue("CognitiveServiceKey");
            Dictionary<string, string> customHeader = new Dictionary<string, string>();
            customHeader.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            HttpClientUtility client = new HttpClientUtility();
            //Request body
            var result = await client.ExecuteGenericAsync(HttpMethod.Post, $"https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment", "", "", customHeader);

            var responseString = await result.Content.ReadAsStringAsync();

            if (result.StatusCode == HttpStatusCode.BadRequest)
            {
                var obj = JsonUtility.GetJObjectFromJsonString(responseString);
                return new ActionResponse(ActionStatus.Success);
            }

            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                var obj = JsonUtility.GetJObjectFromJsonString(responseString);
                return new ActionResponse(ActionStatus.FailureExpected);
            }

            return new ActionResponse(ActionStatus.Failure);

        }
    }
}
