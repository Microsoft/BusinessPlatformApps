using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class DeploySQLScripts : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string connectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");
            string sqlScriptsFolder = request.DataStore.GetValue("SqlScriptsFolder");
            // For database level management scripts, such as enable change tracking will throw exception if transaction is enabled
            var enableTransactionString = request.DataStore.GetValue("enableTransaction");

            bool enableTransaction = true;
            if (!string.IsNullOrWhiteSpace(enableTransactionString))
            {
                bool.TryParse(enableTransactionString, out enableTransaction);
            }

            List<string> files = Directory.EnumerateFiles(Path.Combine(request.Info.App.AppFilePath, sqlScriptsFolder)).ToList();
            foreach (string file in files)
            {
                SqlUtility.InvokeSqlCommand(connectionString, File.ReadAllText(file), new Dictionary<string, string>(), enableTransaction: enableTransaction);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}