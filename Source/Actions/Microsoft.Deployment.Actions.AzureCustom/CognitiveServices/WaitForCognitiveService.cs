using System;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.CognitiveServices
{
    [Export(typeof(IAction))]
    public class WaitForCognitiveService : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var cognitiveServiceKey = request.DataStore.GetValue("CognitiveServiceKey");

            RetryUtility.Retry(10, () =>
            {
                CognitiveServicePayload payloadObj = new CognitiveServicePayload();
                payloadObj.Documents.Add(new Document());

                HttpResponseMessage response;

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", cognitiveServiceKey);
                    HttpContent content = new StringContent(JObject.FromObject(payloadObj).ToString(), System.Text.Encoding.UTF8, "application/json");
                    response = client.PostAsync("https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases", content).Result;
                }

                string result = response.Content.ReadAsStringAsync().Result;
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception();
                }
            });

            return new ActionResponse(ActionStatus.Success);
        }
    }
}