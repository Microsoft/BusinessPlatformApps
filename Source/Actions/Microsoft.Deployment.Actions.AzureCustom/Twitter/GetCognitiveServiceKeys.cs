using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class GetCognitiveServiceKeys : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var cognitiveServiceKey = request.DataStore.GetAllValues("CognitiveServiceKey").LastOrDefault();
            
            if (!string.IsNullOrEmpty(cognitiveServiceKey))
            {
                return new ActionResponse(ActionStatus.Success);
            }

            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation")["Name"].ToString();
            var cognitiveServiceName = request.DataStore.GetValue("CognitiveServiceName");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);        

            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"providers/Microsoft.CognitiveServices/accounts/{cognitiveServiceName}/listKeys", "2016-02-01-preview", string.Empty);
            if (response.IsSuccessStatusCode)
            {
                var subscriptionKeys = JsonUtility.GetJObjectFromJsonString(await response.Content.ReadAsStringAsync());

                JObject newCognitiveServiceKey = new JObject();
                newCognitiveServiceKey.Add("CognitiveServiceKey", subscriptionKeys["key1"].ToString());  
                return new ActionResponse(ActionStatus.Success, newCognitiveServiceKey, true);
            }

            return new ActionResponse(ActionStatus.Failure);
        }
    }
}

