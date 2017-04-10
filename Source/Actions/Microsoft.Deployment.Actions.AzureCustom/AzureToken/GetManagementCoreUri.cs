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
    public class GetManagementCoreUri : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string authBase = Constants.AzureAuthUri;

            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"client_id", Constants.MicrosoftClientId},
                {"prompt", "consent"},
                {"response_type", "code"},
                {"redirect_uri", Uri.EscapeDataString(request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath)},
                {"resource", Uri.EscapeDataString(Constants.AzureManagementCoreApi)}
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