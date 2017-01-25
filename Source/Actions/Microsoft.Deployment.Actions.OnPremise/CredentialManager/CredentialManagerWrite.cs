using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using Simple.CredentialManager;

namespace Microsoft.Deployment.Actions.OnPremise.CredentialManager
{
    [Export(typeof(IAction))]
    public class CredentialManagerWrite : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string targetName = request.DataStore.GetLastValue("CredentialTarget");
            string userName = request.DataStore.GetLastValue("CredentialUsername");
            string password = request.DataStore.GetLastValue("CredentialPassword");

            ActionResponse response = await RequestUtility.CallAction(request, "Microsoft-CredentialManagerDelete");
            if (response.Status == ActionStatus.Failure)
                return response;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
                return new ActionResponse(ActionStatus.Success, new JObject());

            Credential c = new Credential(userName, password, targetName, CredentialType.Generic)
            {
                PersistenceType = PersistenceType.LocalComputer
            };

            if (c.Save())
                return new ActionResponse(ActionStatus.Success, new JObject());
            else
                return new ActionResponse(ActionStatus.Failure, new JObject(),
                    new Win32Exception(Marshal.GetLastWin32Error()), "CredMgrWriteError");
        }
    }
}