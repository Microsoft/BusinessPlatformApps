
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using AzureML.Contract;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;


namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class DeployAzureMLExperiment : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var workspaceName = request.DataStore.GetValue("WorkspaceName");
            var experimentJsonPath = request.DataStore.GetValue("ExperimentJsonPath");
            var experimentName = request.DataStore.GetValue("ExperimentName");

            string json = System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, experimentJsonPath));

            ManagementSDK azuremlClient = new ManagementSDK();
            var workspaces = azuremlClient.GetWorkspacesFromRdfe(azureToken, subscription);
            var workspace = workspaces.SingleOrDefault(p => p.Name.ToLowerInvariant() == workspaceName.ToLowerInvariant());

            if (workspace == null)
            {
                return new ActionResponse(ActionStatus.Failure, null, null, string.Empty, "Workspace not found");
            }

            var workspaceSettings = new WorkspaceSetting()
            {
                AuthorizationToken = workspace.AuthorizationToken.PrimaryToken,
                Location = workspace.Region,
                WorkspaceId = workspace.Id
            };

            var experiment = azuremlClient.GetExperiments(workspaceSettings).LastOrDefault(p => p.Description.ToLowerInvariant() == experimentName.ToLowerInvariant());

            if (experiment != null)
            {
                azuremlClient.SaveExperiment(workspaceSettings, experiment, json);
            }
            else
            {
                experiment = new Experiment()
                {
                    ExperimentId = Guid.NewGuid().ToString(),
                    Description = experimentName,
                    Summary = "Solution Template",
                    Creator = "Solution Template"
                };
            }

            azuremlClient.SaveExperimentAs(workspaceSettings, experiment, json, experimentName);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}