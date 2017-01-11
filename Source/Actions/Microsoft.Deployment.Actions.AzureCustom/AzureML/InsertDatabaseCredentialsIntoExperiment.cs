
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using AzureML.Contract;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;


namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class InsertDatabaseCredentialsIntoExperiment : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var workspaceName = request.DataStore.GetValue("WorkspaceName");
            var experimentName = request.DataStore.GetValue("ExperimentName");

            string sqlIndex = request.DataStore.GetValue("SqlServerIndex") ?? "0";
            var sqlConnectionString = request.DataStore.GetAllValues("SqlConnectionString")[int.Parse(sqlIndex)];
            var sqlCredentials = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);
            
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
            var experiments = azuremlClient.GetExperiments(workspaceSettings);
            var experiment = experiments.LastOrDefault(p => p.Description.ToLowerInvariant() == experimentName.ToLowerInvariant());

            if (experiment == null)
            {
                return new ActionResponse(ActionStatus.Failure, null, null, string.Empty, "Experiment not found");
            }

            string rawJson = string.Empty;
            Experiment exp = azuremlClient.GetExperimentById(workspaceSettings, experiment.ExperimentId, out rawJson);

            rawJson = rawJson.Replace("databaseservertoreplace", sqlCredentials.Server);
            rawJson = rawJson.Replace("databasenametoreplace", sqlCredentials.Database);
            rawJson = rawJson.Replace("databaseusernametoreplace", sqlCredentials.Username);
            rawJson = rawJson.Replace("databasepasswordtoreplace", sqlCredentials.Password);

            azuremlClient.SaveExperiment(workspaceSettings, exp, rawJson);
            return new ActionResponse(ActionStatus.Success);
        }
    }
}