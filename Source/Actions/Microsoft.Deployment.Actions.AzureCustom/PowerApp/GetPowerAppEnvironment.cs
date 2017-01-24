using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class GetPowerAppEnvironment : BaseAction
    {
        private string BASE_POWER_APPS_URL = "https://management.azure.com/providers/Microsoft.PowerApps";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            AzureHttpClient client = new AzureHttpClient(azureToken);

            string environmentBody = "{}";
            string environmentUrl = $"{BASE_POWER_APPS_URL}/environments?api-version=2016-11-01&$filter=minimumAppPermission%20eq%20%27CanEdit%27&$expand=Permissions&_poll=true";

            var environmentsResponse = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, environmentUrl, environmentBody);
            var environmentsString = await environmentsResponse.Content.ReadAsStringAsync();
            var environments = JsonUtility.GetJsonObjectFromJsonString(environmentsString);

            foreach (var environment in environments["value"])
            {
                bool isDefault = false;
                bool.TryParse(environment["properties"]["isDefault"].ToString(), out isDefault);
                if (isDefault && environment["properties"]["permissions"]["CreatePowerApp"] != null)
                {
                    request.DataStore.AddToDataStore("PowerAppEnvironment", environment["name"].ToString(), DataStoreType.Private);
                    return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
                };
            }

            return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "PowerAppNoEnvironment");
            //TODO: Add this error message to common
        }
    }
}