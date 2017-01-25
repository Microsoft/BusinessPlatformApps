using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Common
{
    [Export(typeof(IAction))]
    public class PowerBiLogin : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string tenantId = request.DataStore.GetValue("PowerBITenantId");
            string directory = request.DataStore.GetValue("DirectoryName");

            request.Logger.LogPowerBiLogin(tenantId, directory);

            return new ActionResponse(ActionStatus.Invisible, JsonUtility.GetEmptyJObject());
        }
    }
}
    