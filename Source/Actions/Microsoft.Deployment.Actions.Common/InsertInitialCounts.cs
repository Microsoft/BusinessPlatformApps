using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.Common
{
    [Export(typeof(IAction))]
    public class InsertInitialCounts : BaseAction
    {
        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var connectionString = request.DataStore.GetValue("SqlConnectionString");
            var counts = request.DataStore.GetValue("InitialCounts");
            var schema = request.DataStore.GetValue("SqlSchema");
            var countsObj = JsonConvert.DeserializeObject<Dictionary<string, int>>(counts);

            foreach (var entry in countsObj)
            {
                string cmd = $"INSERT INTO [{schema}].[entityinitialcount] VALUES ('{entry.Key}','{entry.Value}','0','{DateTime.UtcNow.ToString("o")}')";
                SqlUtility.InvokeSqlCommand(connectionString, cmd, new Dictionary<string, string>());
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
