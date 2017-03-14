using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureAS
{
    [Export(typeof(IAction))]
    public class ValidateConnectionToAS : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        { string serverName = request.DataStore.GetValue("ASServerUrl");
            string username = request.DataStore.GetValue("ASAdmin") ??
                              AzureUtility.GetEmailFromToken(request.DataStore.GetJson("AzureToken"));
            string password = request.DataStore.GetValue("ASAdminPassword");

            string connectionString = string.Empty;

            if (serverName.ToLowerInvariant().StartsWith("asazure"))
            {
                connectionString += "Provider=MSOLAP;";
            }

            connectionString += $"Data Source={serverName};";

            if (!string.IsNullOrEmpty(password))
            {
                connectionString +=
                    $"User ID={username};Password={password};Persist Security Info=True; Impersonation Level=Impersonate;";
            }

            try
            {
                Server server = new Server();
                server.Connect(connectionString);
                
                request.DataStore.AddToDataStore("ASConnectionString", connectionString, DataStoreType.Private);
            }
            catch (Exception ex)
            {
              return new ActionResponse(ActionStatus.FailureExpected, null, ex, null);
            }
           
          
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
