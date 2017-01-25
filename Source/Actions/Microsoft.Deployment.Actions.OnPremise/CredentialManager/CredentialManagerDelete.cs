using System;
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
    public class CredentialManagerDelete : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string targetName = request.DataStore.GetLastValue("CredentialTarget");

            Credential c = new Credential()
            {
                Target = targetName,
                Type = CredentialType.Generic,
                PersistenceType = PersistenceType.LocalComputer
            };

            if (c.Exists() && !c.Delete())
            {
                return new ActionResponse(ActionStatus.Failure, new JObject(), new Win32Exception(Marshal.GetLastWin32Error()), "CredMgrReadError");
            }

            return new ActionResponse(ActionStatus.Success, new JObject());
        }
    }
}
