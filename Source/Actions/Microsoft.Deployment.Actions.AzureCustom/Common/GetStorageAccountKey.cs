using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetStorageAccountKey : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var storageAccountName = request.DataStore.GetValue("StorageAccountName");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            var response = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"providers/Microsoft.Storage/storageAccounts/{storageAccountName}/listKeys", "2016-01-01", string.Empty);
            if (response.IsSuccessStatusCode)
            {
                var subscriptionKeys = JsonUtility.GetJObjectFromJsonString(await response.Content.ReadAsStringAsync());
            
                JObject newStorageAccountKey = new JObject();
                newStorageAccountKey.Add("StorageAccountKey", subscriptionKeys["keys"][0]["value"].ToString());
                request.DataStore.AddToDataStore("StorageAccountKey", subscriptionKeys["keys"][0]["value"].ToString());
                return new ActionResponse(ActionStatus.Success, newStorageAccountKey, true);
            }

            
            return new ActionResponse(ActionStatus.Failure);
        }
    }
}
