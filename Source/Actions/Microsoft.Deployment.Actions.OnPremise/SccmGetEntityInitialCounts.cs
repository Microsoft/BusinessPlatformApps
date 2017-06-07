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
using System.Data.SqlClient;

namespace Microsoft.Deployment.Actions.OnPremise
{
    [Export(typeof(IAction))]
    public class SccmGetEntityInitialCounts : BaseAction
    {
        string connectionStringSrc = string.Empty;
        string connectionStringDest = string.Empty;

        public async override Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureSqlDisabled = Convert.ToBoolean(request.DataStore.GetValue("azureSqlDisabled"));
            var sendNotification = Convert.ToBoolean(request.DataStore.GetValue("SendCompletionNotification"));
            
            if(azureSqlDisabled || !sendNotification)
            {
                return new ActionResponse(ActionStatus.Success);
            }            

            connectionStringSrc = request.DataStore.GetAllValues("SqlConnectionString")[0].ToString();
            connectionStringDest = request.DataStore.GetAllValues("SqlConnectionString")[1].ToString();
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

            SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("retention", GetRetentionDays()) };

            DataTable count = SqlUtility.RunCommand(connectionStringSrc, cmd, SqlCommandType.ExecuteWithData);

            return Convert.ToInt32(count.Rows[0][0]);
        }

        private int GetRetentionDays()
        {
            int retentionDays = 120;
            const string cmd = "SELECT [value] FROM pbist_sccm.vw_configuration WHERE [name]='dataretentiondays' AND [configuration group]='SolutionTemplate' AND [configuration subgroup]='System Center'";
            DataTable result = SqlUtility.RunCommand(connectionStringDest, cmd, SqlCommandType.ExecuteWithData);

            if (result!=null && result.Rows.Count>0 && result.Rows[0][0]!= DBNull.Value)
                retentionDays = Convert.ToInt32(result.Rows[0][0]);

            return retentionDays;
        }
    }
}
