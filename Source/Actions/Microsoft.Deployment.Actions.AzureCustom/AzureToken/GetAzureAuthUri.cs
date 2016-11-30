using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureAuthUri : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var aadTenant = request.DataStore.GetValue("AADTenant");

            string authBase;
            string clientId;
            string resource;

            string oauthType = request.DataStore.GetValue("oauthType").ToLowerInvariant();
            switch (oauthType)
            {
                case "mscrm":
                    authBase = Constants.MsCrmAuthority;
                    clientId = Constants.MsCrmClientId;
                    resource = Constants.MsCrmResource;
                    break;
                case "keyvault":
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                    clientId = Constants.MicrosoftClientIdCrm;
                    resource = Constants.AzureManagementApi;
                    break;
                default:
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                    clientId = Constants.MicrosoftClientId;
                    resource = Constants.AzureManagementApi;
                    break;
            }


            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "prompt", "consent" },
                { "response_type", "code" },
                { "redirect_uri", Uri.EscapeDataString(request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath) },
                { "resource", Uri.EscapeDataString(resource) }
            };

            StringBuilder builder = new StringBuilder();
            builder.Append(authBase);
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(builder.ToString()));
        }
    }
}