using System.ComponentModel.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureAuthUriForSPN : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var tenant = request.DataStore.GetFirstValue("SPNTenantId");
            var clientId = request.DataStore.GetFirstValue("SPNAppId");
            string authBase = string.Format(Constants.AzureAuthUri, tenant);
            string authUri = AzureTokenUtility.GetAuthUriForServicePrincipal(clientId, authBase, request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath);

            // hack to allow the SPN to be propagated in AD
            await Task.Delay(50000);
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(authUri.ToString()));
        }
    }
}