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
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var aadTenant = request.DataStore.GetValue("AADTenant");

            string authBase;
            string clientId;
            string resource;
            if (!string.IsNullOrEmpty(request.DataStore.GetValue("IsMsCrm")))
            {
                authBase = Constants.MsCrmAuthority;
                clientId = Constants.MsCrmClientId;
                resource = Constants.MsCrmResource;
            }
            else
            {
                authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                clientId = Constants.MicrosoftClientId;
                resource = Constants.AzureManagementApi;
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