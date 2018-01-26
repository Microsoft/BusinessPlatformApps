using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common;

namespace Microsoft.Deployment.Actions.Custom
{
    [Export(typeof(IAction))]
    public class GetFacebookAuthUri : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string clientId = Constants.FacebookClientId;
            var redirectUri = request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath;
            var authUri = "https://www.facebook.com/v2.10/dialog/oauth?client_id=" + clientId + "&redirect_uri=" + redirectUri + "&response_type=code&scope=manage_pages,pages_show_list,read_insights";
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(authUri.ToString()));
        }
    }
}
