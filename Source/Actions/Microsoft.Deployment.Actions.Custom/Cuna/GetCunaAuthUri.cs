using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Custom.Cuna
{
    [Export(typeof(IAction))]
    public class GetCunaAuthUri : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var url = Constants.CunaTokenUrl;
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(url));
        }
    }
}
