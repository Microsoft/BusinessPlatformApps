using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Site.Service.ServerActions
{
    [Export(typeof(IAction))]
    public class TestServiceAction : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            return new ActionResponse(ActionStatus.Success, "Test");
        }
    }
}