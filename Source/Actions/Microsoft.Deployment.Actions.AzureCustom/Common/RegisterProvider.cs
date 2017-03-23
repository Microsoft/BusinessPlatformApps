using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class RegisterProvider : BaseAction
    {
        private static string REGISTERED = "Registered";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureProvider = request.DataStore.GetLastValue("AzureProvider");
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscriptionId = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscriptionId, azureToken);

            using (ResourceManagementClient managementClient = new ResourceManagementClient(creds))
            {

                var prov = await managementClient.Providers.GetAsync(azureProvider);
                if (!prov.Provider.RegistrationState.EqualsIgnoreCase((REGISTERED)))
                {
                    AzureOperationResponse operationResponse = managementClient.Providers.Register(azureProvider);
                    if (
                        !(operationResponse.StatusCode == System.Net.HttpStatusCode.OK ||
                          operationResponse.StatusCode == System.Net.HttpStatusCode.Accepted))
                    {
                        return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "RegisterProviderError");
                    }

                    // Temporary hack to wait for registration to complete
                    await Task.Delay(20000);
                }
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}