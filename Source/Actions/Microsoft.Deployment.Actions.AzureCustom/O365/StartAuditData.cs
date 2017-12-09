using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.O365
{
    [Export(typeof(IAction))]
    public class StartAuditData : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var tenant = request.DataStore.GetFirstValue("SPNTenantId");
            var clientId = request.DataStore.GetFirstValue("SPNAppId");
            var clientSecret = request.DataStore.GetFirstValue("SPNKey");

            string requestUrl = $"https://manage.office.com/api/v1.0/{tenant}/activity/feed/subscriptions/start?contentType=Audit.General&PublisherIdentifier={Guid.NewGuid()}";
            string resourceUri = "https://manage.office.com";
            string redirectUri = request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath;
            string authorityUri = $"https://login.windows.net/{tenant}/oauth2/authorize";

            AuthenticationContext authContext = new AuthenticationContext(authorityUri);
            ClientCredential cred = new ClientCredential(clientId, clientSecret);
            AuthenticationResult token = await authContext.AcquireTokenAsync(resourceUri, cred);

            AzureHttpClient client = new AzureHttpClient(token.AccessToken);
            var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, requestUrl, "");

            if(response.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Success);
            }

            return new ActionResponse(ActionStatus.Failure);
        }
    }
}
