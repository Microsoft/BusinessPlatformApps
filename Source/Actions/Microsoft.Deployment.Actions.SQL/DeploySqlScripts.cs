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

            List<string> files = Directory.EnumerateFiles(Path.Combine(request.Info.App.AppFilePath, sqlScriptsFolder)).ToList();
            foreach (string file in files)
            {
                SqlUtility.InvokeSqlCommand(connectionString, File.ReadAllText(file), new Dictionary<string, string>());
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}