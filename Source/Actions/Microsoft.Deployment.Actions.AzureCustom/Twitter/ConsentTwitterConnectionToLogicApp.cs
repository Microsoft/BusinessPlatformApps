using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class ConsentTwitterConnectionToLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var twitterCode = request.DataStore.GetValue("TwitterCode");
            
            dynamic payload = new ExpandoObject();
            payload.objectId = null;
            payload.tenantId = null;
            payload.code = twitterCode;

            HttpResponseMessage consent = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post,
                "/providers/Microsoft.Web/connections/twitter/confirmConsentCode", "2015-08-01-preview", 
                JsonUtility.GetJsonStringFromObject(payload));

            var consentInformation = JsonUtility.GetJObjectFromJsonString(await consent.Content.ReadAsStringAsync());
            if (!consent.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, consentInformation, null , DefaultErrorCodes.DefaultErrorCode,  "Failed to create connection");
            }

            payload = new ExpandoObject();
            payload.properties = new ExpandoObject();
            payload.properties.displayName = "twitter";
            payload.properties.api = new ExpandoObject();
            payload.properties.api.id = $"subscriptions/{subscription}/providers/Microsoft.Web/locations/{location}/managedApis/twitter";
            payload.location = location;

            HttpResponseMessage connection = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Put,
                "/providers/Microsoft.Web/connections/twitter", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

            if (!connection.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()),null, 
                    DefaultErrorCodes.DefaultErrorCode, "Failed to create connection");
            }

            var connectionData = JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync());
            if (connectionData["properties"]["statuses"][0]["status"].ToString() != "Connected")
            {
                return new ActionResponse(ActionStatus.Failure, connectionData);
            }

            return new ActionResponse(ActionStatus.Success, connectionData);
        }
    }
}