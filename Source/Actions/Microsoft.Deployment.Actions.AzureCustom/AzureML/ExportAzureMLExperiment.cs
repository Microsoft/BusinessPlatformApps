using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using AzureML;
using AzureML.Contract;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class ExportAzureMLExperiment : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var workspaceId = request.DataStore.GetValue("WorkspaceId");
            var workspaceToken = request.DataStore.GetValue("WorkspaceToken");
            var workspaceRegion = request.DataStore.GetValue("WorkspaceRegion");

            ManagementSDK azuremlClient = new ManagementSDK();

            var workspaceSettings = new WorkspaceSetting()
            {
                AuthorizationToken = workspaceToken,
                Location = workspaceRegion,
                WorkspaceId = workspaceId
            };

            var workspace = azuremlClient.GetWorkspaceFromAmlRP(workspaceSettings);
         
            if (workspace == null)
            {
                return new ActionResponse(ActionStatus.Failure, null, null, string.Empty, "Workspace not found");
            }


            var experiments = azuremlClient.GetExperiments(workspaceSettings);
            var jsonObj = new Dictionary<string, string>();
            foreach (var experiment in experiments)
            {
                string rawjson = string.Empty;
                azuremlClient.GetExperimentById(workspaceSettings, experiment.ExperimentId, out rawjson);
                jsonObj.Add(experiment.Description, rawjson);
            }

            return new ActionResponse(ActionStatus.Success, jsonObj);
        }
    }
}