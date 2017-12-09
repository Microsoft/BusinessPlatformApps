using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class SetConfigValueInSql : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            // Provided by the json 
            string configTable = request.DataStore.GetValue("SqlConfigTable");

            // Provided by the user including the messages below
            string connectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");

            // Get list of settings to deploy;
            var listGroup = request.DataStore.GetAllJson("SqlGroup");
            var listSubgroup = request.DataStore.GetAllJson("SqlSubGroup");
            var listConfigEntryName = request.DataStore.GetAllJson("SqlEntryName");
            var listConfigEntryValue = request.DataStore.GetAllJson("SqlEntryValue");

            // This action should not be called with incomplete entries - most likely an init.json error
            if (listGroup == null || listSubgroup == null || listConfigEntryName == null || listConfigEntryValue == null)
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "SQL_MissingConfigValues");

            // Counts must be consistent
            if (!(listGroup.Count == listSubgroup.Count && listSubgroup.Count == listConfigEntryName.Count && listConfigEntryName.Count == listConfigEntryValue.Count))
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "SQL_MalformedConfigValues");

            for (int i = 0; i < listGroup.Count; i++)
            {
                string group = listGroup[i].ToString();
                string subgroup = listSubgroup[i].ToString();
                string configEntryName = listConfigEntryName[i].ToString();
                string configEntryValue = listConfigEntryValue[i].ToString();

                string query = string.Format(queryTemplate, configTable, group, subgroup, configEntryName, configEntryValue);

                SqlUtility.InvokeSqlCommand(connectionString, query, null);
            }

            return new ActionResponse(ActionStatus.Success);
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