
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using AzureML.Contract;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using Newtonsoft.Json.Linq;


namespace Microsoft.Deployment.Actions.AzureCustom.AzureML
{
    [Export(typeof(IAction))]
    public class InsertDatabaseCredentialsIntoExperiment : BaseAction
    {
        private SqlCredentials sqlCredentials;

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var workspaceName = request.DataStore.GetValue("WorkspaceName");
            var experimentName = request.DataStore.GetValue("ExperimentName");

            string sqlConnectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");
            sqlCredentials = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);

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
            rawJson = this.ReplaceConnectionString(rawJson);
            azuremlClient.SaveExperiment(workspaceSettings, exp, rawJson);
            return new ActionResponse(ActionStatus.Success);
        }

        private string ReplaceConnectionString(string json)
        {
            JObject obj = JsonUtility.GetJObjectFromJsonString(json);
            var moduleNodes = obj.SelectToken("Graph").SelectToken("ModuleNodes");
            foreach (var module in moduleNodes)
            {
                var parameters = module.SelectToken("ModuleParameters");

                foreach (var param in parameters)
                {
                    if (param["Name"].ToString() == "Database Server Name")
                    {
                        param["Value"] = this.sqlCredentials.Server;
                    }

                    if (param["Name"].ToString() == "Database Name")
                    {
                        param["Value"] = this.sqlCredentials.Database;
                    }

                    if (param["Name"].ToString() == "Server User Account Name")
                    {
                        param["Value"] = this.sqlCredentials.Username;
                    }

                    if (param["Name"].ToString() == "Server User Account Password")
                    {
                        param["Value"] = this.sqlCredentials.Password;
                    }
                }
            }

            return obj.ToString();
        }
    }
}