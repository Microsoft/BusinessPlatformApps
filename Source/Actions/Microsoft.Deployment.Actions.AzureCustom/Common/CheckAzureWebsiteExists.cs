using System.ComponentModel.Composition;
using System.Dynamic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class CheckAzureWebsiteExists : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var sitename = request.DataStore.GetJson("SiteName");

            dynamic obj = new ExpandoObject();
            obj.hostName = ".azurewebsites.net";
            obj.siteName = sitename;
            obj.subscriptionId = subscription;

            AzureHttpClient client = new AzureHttpClient(token, subscription);
            var statusResponse = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, @"https://web1.appsvcux.ext.azure.com/websites/api/Websites/ValidateSiteName",
               JsonUtility.GetJsonStringFromObject(obj));
            var response = await statusResponse.Content.ReadAsStringAsync();
            if (!statusResponse.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.FailureExpected, JsonUtility.GetJsonObjectFromJsonString(response), null,  AzureErrorCodes.AzureWebsiteNameTaken);
            }

            return new ActionResponse(ActionStatus.Success, response);
        }
    }
}
