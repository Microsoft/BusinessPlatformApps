using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.OnPremise
{
    [Export(typeof(IAction))]
    public class SccmGetEntityInitialCounts : BaseAction
    {
        string connectionString = string.Empty;

        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureSqlDisabled = Convert.ToBoolean(request.DataStore.GetValue("azureSqlDisabled"));
            var sendNotification = Convert.ToBoolean(request.DataStore.GetValue("SendCompletionNotification"));
            
            if(azureSqlDisabled || !sendNotification)
            {
                return new ActionResponse(ActionStatus.Success);
            }            

            connectionString = request.DataStore.GetAllValues("SqlConnectionString")[0].ToString();
            string rootPath = request.Info.App.AppFilePath + @"\Service\Counts\";
            var entities = Directory.EnumerateFiles(rootPath);

            Dictionary<string, int> initialCounts = new Dictionary<string, int>();

            foreach(var entity in entities)
            {
                string table = entity.Split('\\').Last().Split('.').First().ToLowerInvariant();
                int count = GetCount(entity);
                initialCounts.Add(table, count);
            }

            request.DataStore.AddToDataStore("InitialCounts", JsonUtility.GetJObjectFromObject(initialCounts));
            return new ActionResponse(ActionStatus.Success);
        }

        private int GetCount(string entity)
        {
            var cmd = File.ReadAllText(entity);

            DataTable count = SqlUtility.RunCommand(connectionString, cmd, SqlCommandType.ExecuteWithData);

            return Convert.ToInt32(count.Rows[0].ItemArray.First());
        }
    }
}
