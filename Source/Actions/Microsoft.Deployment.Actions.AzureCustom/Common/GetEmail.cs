using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class GetEmail : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            string admin = request.DataStore.GetValue("ASAdmin") ??
                AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken"));

            var obj = JsonUtility.GetJObjectFromStringValue(admin);
            return new ActionResponse(ActionStatus.Success, obj);
        }
    }
}
