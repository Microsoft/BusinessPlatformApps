using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class SetConfigValueInSql : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            // Provided by the json 
            var sqlIndex = int.Parse(request.DataStore.GetValue("SqlServerIndex"));
            string configTable = request.DataStore.GetValue("SqlConfigTable");


            // Provided by thge user including the messages below
            string connectionString = request.DataStore.GetAllValues("SqlConnectionString")[sqlIndex].ToString();
                // Must specify Initial Catalog

            // Get list of settings to deploy;
            var listGroup = request.DataStore.GetAllJson("SqlGroup");
            var listSubgroup = request.DataStore.GetAllJson("SqlSubGroup");
            var listConfigEntryName = request.DataStore.GetAllJson("SqlEntryName");
            var listConfigEntryValue = request.DataStore.GetAllJson("SqlEntryValue");

            if (listGroup == null || listSubgroup == null || listConfigEntryName == null || listConfigEntryValue == null)
            {
                return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject(),null, DefaultErrorCodes.DefaultErrorCode, 
                    "Configuration value properties not found");
            }

            for (int i = 0; i < listGroup.Count(); i++)
            {
                string group = request.DataStore.GetAllJson("SqlGroup")[i].ToString();
                string subgroup = request.DataStore.GetAllJson("SqlSubGroup")[i].ToString();
                string configEntryName = request.DataStore.GetAllJson("SqlEntryName")[i].ToString();
                string configEntryValue = request.DataStore.GetAllJson("SqlEntryValue")[i].ToString();

                string query = string.Format(queryTemplate, configTable, group, subgroup, configEntryName,
                    configEntryValue);

                SqlUtility.InvokeSqlCommand(connectionString, query, null);
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }

        private const string queryTemplate = @"MERGE {0} AS t  
                                           USING ( VALUES('{1}', '{2}', '{3}', '{4}') ) AS s(configuration_group, configuration_subgroup, [name], [value])
                                           ON t.configuration_group=s.configuration_group AND t.configuration_subgroup=s.configuration_subgroup AND t.[name]=s.[name]
                                           WHEN matched THEN
                                               UPDATE SET [value]=s.[value]
                                           WHEN NOT matched THEN
                                               INSERT (configuration_group, configuration_subgroup, [name], [value]) VALUES (s.configuration_group, s.configuration_subgroup, s.[name], s.[value]);";

    }
}