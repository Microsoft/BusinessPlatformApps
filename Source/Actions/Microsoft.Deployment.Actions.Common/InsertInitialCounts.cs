using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Common
{
    [Export(typeof(IAction))]
    public class InsertInitialCounts : BaseAction
    {
        private const string commandTemplate = "INSERT INTO {0}.entityinitialcount VALUES(@p1, @p2, @p3, @p4)";

        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string connectionString = request.DataStore.GetValue("SqlConnectionString");
            string counts = request.DataStore.GetValue("InitialCounts");
            string schema = request.DataStore.GetValue("SqlSchema");
            Dictionary<string, int> countsObj = JsonConvert.DeserializeObject<Dictionary<string, int>>(counts);
            
            schema = SqlUtility.SanitizeSchemaName(schema);
            string statement = string.Format(commandTemplate, schema);
            DateTime timeStamp = DateTime.UtcNow;
            foreach (var entry in countsObj)
            {
                SqlParameter[] parameters = SqlUtility.MapValuesToSqlParameters(entry.Key, entry.Value, 0, timeStamp);
                SqlUtility.ExecuteQueryWithParameters(connectionString, statement, parameters);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
