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

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class CreateConnectorToLogicApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var connectorName = request.DataStore.GetValue("ConnectorName");
            var requiresConsent = request.DataStore.GetValue("RequiresConsent");

            //Needs to be changed once Logic Apps makes it available
            if (connectorName == "bingsearch")
            {
                location = "brazilsouth";
            }

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            var registeration = await client.Providers.RegisterAsync("Microsoft.Web");

            dynamic payload = new ExpandoObject();
            payload.properties = new ExpandoObject();
            payload.properties.parameterValues = new ExpandoObject();
            payload.properties.parameterValues.sku = "Enterprise";
            payload.properties.displayName = connectorName;
            payload.properties.api = new ExpandoObject();
            payload.properties.api.id = $"subscriptions/{subscription}/providers/Microsoft.Web/locations/{location}/managedApis/{connectorName}";
            payload.location = location;

            HttpResponseMessage connection = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Put,
                $"/providers/Microsoft.Web/connections/{connectorName}", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

            if (!connection.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()),
                    null, DefaultErrorCodes.DefaultErrorCode, "Failed to create connection");
            }

            var redirectRoot = request.Info.WebsiteRootUrl;

            if (requiresConsent != null && requiresConsent.ToLowerInvariant() == "true")
            {
                // Get Consent links for auth
                payload = new ExpandoObject();
                payload.parameters = new ExpandoObject[1];
                payload.parameters[0] = new ExpandoObject();
                payload.parameters[0].objectId = null;
                payload.parameters[0].tenantId = null;
                payload.parameters[0].parameterName = "token";
                payload.parameters[0].redirectUrl = redirectRoot + Constants.WebsiteRedirectPath;

                HttpResponseMessage consent = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Post,
                    $"/providers/Microsoft.Web/connections/{connectorName}/listConsentLinks", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

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

            return new ActionResponse(ActionStatus.Success);
        }
    }
}