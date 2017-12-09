using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Http;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class ValidateCognitiveKey : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var subscriptionKey = request.DataStore.GetValue("CognitiveServiceKey");

            AzureHttpClient client = new AzureHttpClient(new Dictionary<string, string>()
            {
                { "Ocp-Apim-Subscription-Key", subscriptionKey }
            });

            HttpResponseDetails response = await client.GetJsonDetails(HttpMethod.Post, $"https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment");

            if (response.Code == HttpStatusCode.BadRequest)
            {
                var obj = JsonUtility.GetJObjectFromJsonString(response.Json);
                return new ActionResponse(ActionStatus.Success);
            }

            if (response.Code == HttpStatusCode.Unauthorized)
            {
                var obj = JsonUtility.GetJObjectFromJsonString(response.Json);
                return new ActionResponse(ActionStatus.FailureExpected);
            }

            return new ActionResponse(ActionStatus.Failure);
        }
    }
}