using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class DeployAzureASModel : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string xmla = request.DataStore.GetValue("xmlaFilePath");
            string asDatabase = request.DataStore.GetValue("ASDatabase");
            string sqlConnectionString = request.DataStore.GetValue("SqlConnectionString");
            var connectionStringObj = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);
            string connectionString = request.DataStore.GetValue("ASConnectionString");

            string xmlaContents = File.ReadAllText(request.Info.App.AppFilePath + "/" + xmla);

            using (Server server = new Server())
            {
                try
                {
                    server.Connect(connectionString);

                    // Delete existing
                    Database db = server.Databases.FindByName(asDatabase);
                    db?.Drop();

                    // Deploy database definition
                    XmlaResultCollection response = server.Execute(xmlaContents);
                    if (response.ContainsErrors)
                    {
                        return new ActionResponse(ActionStatus.Failure, response[0].Value);
                    }

                    // Reload metadata and update connection string
                    server.Refresh(true);
                    db = server.Databases.FindByName(asDatabase);
                    ((ProviderDataSource)db.Model.DataSources[0]).ConnectionString = $"Provider=SQLNCLI11;Data Source=tcp:{connectionStringObj.Server};Persist Security Info=True;User ID={connectionStringObj.Username};Password={connectionStringObj.Password};Initial Catalog={connectionStringObj.Database}";

                    db.Update(UpdateOptions.ExpandFull);
                }
                catch (Exception e)
                {
                    return new ActionResponse(ActionStatus.Failure, string.Empty, e, null, "AS Database was not deployed");
                }

                server.Disconnect(true);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}