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
            var sqlScriptsFolder = request.DataStore.GetValue("SqlScriptsFolder");

            string connectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");

            var files = Directory.EnumerateFiles(Path.Combine(request.Info.App.AppFilePath, sqlScriptsFolder)).ToList();
            foreach (var f in files)
            {
                SqlUtility.InvokeSqlCommand(connectionString, File.ReadAllText(f), new Dictionary<string, string>());
            }
            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}