using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class CreateTwitterConnectionToLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");


            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            var registeration = await client.Providers.RegisterAsync("Microsoft.Web");

            dynamic payload = new ExpandoObject();
            payload.properties = new ExpandoObject();
            payload.properties.displayName = "twitter";
            payload.properties.api = new ExpandoObject();
            payload.properties.api.id = $"subscriptions/{subscription}/providers/Microsoft.Web/locations/{location}/managedApis/twitter";
            payload.location = location;

            HttpResponseMessage connection = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Put,
                "/providers/Microsoft.Web/connections/twitter", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

            if (!connection.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()), 
                    null, DefaultErrorCodes.DefaultErrorCode, "Failed to create connection");
            }

            
            // Get Consent links for auth
            payload = new ExpandoObject();
            payload.parameters = new ExpandoObject[1];
            payload.parameters[0] = new ExpandoObject();
            payload.parameters[0].objectId = null;
            payload.parameters[0].tenantId = null;
            payload.parameters[0].parameterName = "token";
            payload.parameters[0].redirectUrl = "https://bpsolutiontemplates.com" + Constants.WebsiteRedirectPath;

            HttpResponseMessage consent = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post,
                "/providers/Microsoft.Web/connections/twitter/listConsentLinks", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

            if (!consent.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()),
                    null, DefaultErrorCodes.DefaultErrorCode, "Failed to get consent");
            }
            var connectiondata = JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync());
            var consentdata = JsonUtility.GetJObjectFromJsonString(await consent.Content.ReadAsStringAsync());
            dynamic objectToReturn = new ExpandoObject();
            objectToReturn.Consent = consentdata;
            objectToReturn.Connection = connectiondata;
            objectToReturn.UniqueId = payload.parameters[0].objectId;

            return new ActionResponse(ActionStatus.Success, objectToReturn);
        }
    }
}