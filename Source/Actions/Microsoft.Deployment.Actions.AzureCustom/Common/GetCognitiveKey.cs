using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetCognitiveKey : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string keyNumber = request.DataStore.GetValue("KeyNumber") ?? "0";
            int keyNumberParsed = int.Parse(keyNumber);

            var cognitiveServiceKeys = request.DataStore.GetAllValues("CognitiveServiceKey");

            if (cognitiveServiceKeys.Count - 1 >= keyNumberParsed)
            {
                string key = cognitiveServiceKeys[keyNumberParsed];

                if (!string.IsNullOrEmpty(key))
                {
                    return new ActionResponse(ActionStatus.Success);
                }
            }

            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var cognitiveServiceName = request.DataStore.GetValue("CognitiveServiceName");
            var cognitiveServiceType = request.DataStore.GetValue("CognitiveServiceType");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"providers/Microsoft.CognitiveServices/accounts/{cognitiveServiceName}/listKeys", "2016-02-01-preview", string.Empty);
            if (response.IsSuccessStatusCode)
            {
                var subscriptionKeys = JsonUtility.GetJObjectFromJsonString(await response.Content.ReadAsStringAsync());

                JObject newCognitiveServiceKey = new JObject();
                newCognitiveServiceKey.Add("CognitiveServiceKey", subscriptionKeys["key1"].ToString());
                string cognitiveKey = subscriptionKeys["key1"].ToString();

                var itemsInDataStore = request.DataStore.GetAllDataStoreItems("CognitiveServiceKey");
                if(itemsInDataStore.Count - 1 >= keyNumberParsed)
                {
                    request.DataStore.UpdateValue(itemsInDataStore[keyNumberParsed].DataStoreType, itemsInDataStore[keyNumberParsed].Route, itemsInDataStore[keyNumberParsed].Key, cognitiveKey);
                }
                else
                {
                    request.DataStore.AddToDataStore("CognitiveServiceKey", cognitiveKey);
                }

                return new ActionResponse(ActionStatus.Success, newCognitiveServiceKey, true);
            }

            return new ActionResponse(ActionStatus.Failure);
        }
    }
}

