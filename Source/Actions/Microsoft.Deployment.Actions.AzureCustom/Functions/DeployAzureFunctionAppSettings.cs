using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployAzureFunctionAppSettings : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var sitename = request.DataStore.GetValue("FunctionName");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            var existingResponse = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post, $"/providers/Microsoft.Web/sites/{sitename}/config/appSettings/list", "2016-03-01", "{}");

            if(!existingResponse.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure);
            }

            var appSettingsPayload = JObject.Parse(await existingResponse.Content.ReadAsStringAsync());
            JObject properties = appSettingsPayload["properties"] as JObject;

            if (request.DataStore.GetJson("AppSettingKeys") != null)
            {
                foreach (var item in request.DataStore.GetJson("AppSettingKeys"))
                {
                    string key = item.Path.Split('.').Last();
                    string value = (string)item;
                    properties.Add(key, value);
                }
            }

            var appSettingCreated = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Put, $"/providers/Microsoft.Web/sites/{sitename}/config/appSettings", "2016-03-01", appSettingsPayload.ToString());
            var response = await appSettingCreated.Content.ReadAsStringAsync();
            if (!appSettingCreated.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(response), null, DefaultErrorCodes.DefaultErrorCode, "Error creating appsetting");
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}