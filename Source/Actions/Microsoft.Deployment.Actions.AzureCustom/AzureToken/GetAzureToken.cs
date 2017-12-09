using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string code = request.DataStore.GetValue("code");
            string aadTenant = request.DataStore.GetValue("AADTenant");
            string oauthType = (request.DataStore.GetValue("oauthType") ?? string.Empty).ToLowerInvariant();
            JObject token = new JObject();

            token = oauthType == "mscrm" ? AzureTokenUtility.GetTokenForResourceFromCode(Constants.AzureManagementCoreApi, Constants.MsCrmClientId, aadTenant, request.Info.WebsiteRootUrl, code) :
                                           AzureTokenUtility.GetTokenForResourceFromCode(oauthType, aadTenant, request.Info.WebsiteRootUrl, code);

            if (token.SelectToken("error") != null)
            {
                return new ActionResponse(ActionStatus.Failure, token, null, DefaultErrorCodes.DefaultLoginFailed, token.SelectToken("error_description")?.ToString());
            }

            var emailAddress = AzureUtility.GetEmailFromToken(token);
            if (emailAddress.Contains('#'))
            {
                emailAddress = emailAddress.Split('#')?[1];
            }
            request.DataStore.AddToDataStore("EmailAddress", emailAddress);

            switch (oauthType)
            {
                case "keyvault":
                    request.DataStore.AddToDataStore("AzureTokenKV", token);
                    break;
                case "as":
                    request.DataStore.AddToDataStore("AzureTokenAS", token);
                    break;
                case "axerp":
                    request.DataStore.AddToDataStore("AxToken", token);
                    break;
                case "mscrm":
                    JObject crmToken = AzureTokenUtility.GetTokenForResourceFromExistingToken(oauthType, request.Info.WebsiteRootUrl, token, Constants.MsCrmResource);
                    request.DataStore.AddToDataStore("MsCrmToken", crmToken);
                    request.DataStore.AddToDataStore("AzureToken", token);
                    break;
                case "powerbi":
                    request.DataStore.AddToDataStore("PBIToken", token);
                    request.DataStore.AddToDataStore("DirectoryName", emailAddress.Split('@').Last());
                    request.DataStore.AddToDataStore("PowerBITenantId", AzureUtility.GetTenantFromToken(token));
                    break;
                case "o365":
                    request.DataStore.AddToDataStore("O365Token", token);
                    request.DataStore.AddToDataStore("DirectoryName", emailAddress.Split('@').Last());
                    request.DataStore.AddToDataStore("PowerBITenantId", AzureUtility.GetTenantFromToken(token));
                    break;
                default:
                    request.DataStore.AddToDataStore("AzureToken", token);
                    var tenantId = AzureUtility.GetTenantFromToken(token);
                    var directoryName = emailAddress.Split('@').Last();
                    request.DataStore.AddToDataStore("DirectoryName", directoryName);
                    request.DataStore.AddToDataStore("PowerBITenantId", tenantId);
                    break;
            }

            return new ActionResponse(ActionStatus.Success, token, true);
        }
    }
}