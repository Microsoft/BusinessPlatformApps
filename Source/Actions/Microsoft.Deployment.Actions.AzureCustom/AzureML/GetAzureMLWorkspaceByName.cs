
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;


namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class GetAzureMLWorkspaceByName : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var workspaceName = request.DataStore.GetValue("WorkspaceName");

            ManagementSDK azuremlClient = new ManagementSDK();
            var workspaces = azuremlClient.GetWorkspacesFromRdfe(azureToken, subscription);
            var workspace = workspaces.SingleOrDefault(p => p.Name.ToLowerInvariant() == workspaceName.ToLowerInvariant());

            if (workspace != null)
            {
                return new ActionResponse(ActionStatus.Success, workspace, true);
            }

            return new ActionResponse(ActionStatus.Failure);
        }
    }
}