//-----------------------------------------------------------------------
// <copyright file="GetCMDEntities.cs" company="Microsoft Corp.">
//     Copyright (c) Microsoft Corp. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.CDM
{
    using System.ComponentModel.Composition;
    using System.Threading;
    using Microsoft.Azure;
    using Microsoft.Azure.Management.Resources;
    using Microsoft.Azure.Management.Resources.Models;
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.ErrorCode;
    using Microsoft.Deployment.Common.Helpers;
    using System.Net.Http;

    [Export(typeof(IAction))]
    public class ProvisionCDM : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");

            var createEnvironment = await RequestUtility.CallAction(request,"Microsoft-CreateEnvironment");
            var createDatabase = await RequestUtility.CallAction(request, "Microsoft-CreateDatabase");

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
