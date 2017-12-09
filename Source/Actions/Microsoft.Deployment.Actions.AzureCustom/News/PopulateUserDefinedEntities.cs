using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Custom;

namespace Microsoft.Deployment.Actions.AzureCustom.News
{
    [Export(typeof(IAction))]
    class PopulateUserDefinedEntities : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            List<NewsEntity> userEntities = JsonUtility.Deserialize<List<NewsEntity>>(request.DataStore.GetValue("UserDefinedEntities"));

            if (userEntities != null)
            {
                var sqlConn = request.DataStore.GetValue("SqlConnectionString");

                var entityTypes = entityTypesTable();
                var userEntityTable = userEntitiesTable();

                foreach (NewsEntity userEntity in userEntities)
                {

                    DataRow entityTypeRow = entityTypes.NewRow();
                    entityTypeRow["icon"] = userEntity.Icon;
                    entityTypeRow["color"] = userEntity.Color;
                    entityTypeRow["entityType"] = userEntity.Name;
                    entityTypes.Rows.Add(entityTypeRow);

                    List<string> values = new List<string>(
                               userEntity.Values.Split(new string[] { "\n" },
                               System.StringSplitOptions.RemoveEmptyEntries));

                    foreach (string value in values)
                    {
                        DataRow userEntityRow = userEntityTable.NewRow();
                        userEntityRow["regex"] = value ;
                        userEntityRow["entityValue"] = value;
                        userEntityRow["entityType"] = userEntity.Name;
                        userEntityTable.Rows.Add(userEntityRow);
                    }
                }

                BulkInsert(sqlConn, entityTypes, "bpst_news.typedisplayinformation");
                BulkInsert(sqlConn, userEntityTable, "bpst_news.userdefinedentitydefinitions");
            }

            return new ActionResponse(ActionStatus.Success);
        }


        public static void BulkInsert(string connString, DataTable table, string tableName)
        {
            try
            {
                using (SqlBulkCopy bulk = new SqlBulkCopy(connString))
                {
                    bulk.BatchSize = 1000;
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer(table);
                    bulk.Close();
                }
            }
            catch
            {
                throw new Exception("overflow during batch insert in table " + tableName);
            }
        }

        public static DataTable userEntitiesTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("regex");
            table.Columns.Add("entityType");
            table.Columns.Add("entityValue");
            return table;
        }

        public static DataTable entityTypesTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("entityType");
            table.Columns.Add("icon");
            table.Columns.Add("color");
            return table;
        }
    }
}