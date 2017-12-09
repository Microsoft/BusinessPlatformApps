using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.ApiManagement;
using Microsoft.Deployment.Common.Model.Bpst;

namespace Microsoft.Deployment.Actions.AzureCustom.APIManagement
{
    [Export(typeof(IAction))]
    public class GetApiManagementServices : BaseAction
    {
        private const string SUCCEEDED = "Succeeded";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            BpstAzure ba = new BpstAzure(request.DataStore);

            AzureHttpClient ahc = new AzureHttpClient(ba.TokenAzure);

            string url = $"https://management.azure.com/subscriptions/{ba.IdSubscription}/providers/Microsoft.ApiManagement/service?api-version=2016-10-10";

            List<ApiManagementService> apiManagementServices = await ahc.RequestValue<List<ApiManagementService>>(HttpMethod.Get, url);

            List<ApiManagementService> activeApiManagementServices = new List<ApiManagementService>();

            if (!apiManagementServices.IsNullOrEmpty())
            {
                activeApiManagementServices = apiManagementServices.FindAll(service => IsServiceActive(service));
            }

            return activeApiManagementServices.IsNullOrEmpty()
                ? new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("ApiManagementErrorNoServicesFound"))
                : new ActionResponse(ActionStatus.Success, JsonUtility.Serialize(activeApiManagementServices));
        }

        private bool IsServiceActive(ApiManagementService service)
        {
            return service != null && service.Properties != null && !string.IsNullOrEmpty(service.Properties.ProvisioningState) && service.Properties.ProvisioningState.EqualsIgnoreCase(SUCCEEDED);
        }
    }
}