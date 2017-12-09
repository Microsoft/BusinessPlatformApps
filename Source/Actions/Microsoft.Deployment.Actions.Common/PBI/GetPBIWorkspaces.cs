using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PBI;

namespace Microsoft.Deployment.Actions.Common.PBI
{
    [Export(typeof(IAction))]
    public class GetPBIWorkspaces : BaseAction
    {
        private const string PBI_DEFAULT_WORKSPACE = "My workspace";
        private const string PBI_ENDPOINT_GROUPS = "v1.0/myorg/groups";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            AzureHttpClient client = new AzureHttpClient(request.DataStore.GetJson("PBIToken", "access_token"));

            PBIWorkspaces pbiWorkspaces = await client.Request<PBIWorkspaces>(HttpMethod.Get, request.DataStore.GetValue("PBIClusterUri") + PBI_ENDPOINT_GROUPS);

            pbiWorkspaces.Workspaces.Insert(0, new PBIWorkspace(PBI_DEFAULT_WORKSPACE));

            return new ActionResponse(ActionStatus.Success, JsonUtility.Serialize<List<PBIWorkspace>>(pbiWorkspaces.Workspaces));
        }
    }
}