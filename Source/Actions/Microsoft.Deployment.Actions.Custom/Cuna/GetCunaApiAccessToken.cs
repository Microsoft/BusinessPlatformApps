using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Custom.Cuna
{
    [Export(typeof(IAction))]
    public class GetCunaApiAccessToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            try
            {
                var authority = String.Format(CultureInfo.InvariantCulture, Constants.CunaApiAadInstance, Constants.CunaApiAadTenantId);

                var clientCredential = new ClientCredential(Constants.CunaApiAadClientId, Constants.CunaApiAadSecret);
                var authContext = new AuthenticationContext(authority);
                var result = await authContext.AcquireTokenAsync(Constants.CunaApiAadResourceId, clientCredential);

                request.DataStore.AddToDataStore("CunaApiAccessToken", result.AccessToken, DataStoreType.Private);
                return new ActionResponse(ActionStatus.Success);
            }
            catch(Exception ex)
            {
                return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("CunaGetApiAccessTokenFailed", ex.Message));
            }
        }
    }
}
