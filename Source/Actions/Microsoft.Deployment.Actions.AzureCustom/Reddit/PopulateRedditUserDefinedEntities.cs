using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Custom;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    class PopulateRedditUserDefinedEntities : BaseAction
    {
        // there has got to be a better way to specify colors like this
        private static readonly List<string> predefinedColors = new List<string>
        {
            "#01B8AA",
            "#FD625E",
            "#F2C80F",
            "#8AD4EB",
            "#FE9666", 
            "#A66999",
            "#3599B8",
            "#DFBFBF",
            "#4AC5BB",
            "#5F6B6D",
            "#FB8281",
            "#F4D25A",
            "#7F898A",
            "#A4DDEE",
            "#FDAB89",
            "#B687AC",
            "#28738A",
            "#A78F8F",
            "#168980",
            "#293537",
            "#BB4A4A",
            "#B59525",
            "#475052",
            "#6A9FB0",
            "#BD7150",
            "#7B4F71",
            "#1B4D5C",
            "#706060",
            "#0F5C55",
            "#1C2325",
            "#7D3231",
            "#796419",
            "#303637"
        };

        // we can only have 32 aliases max across all entities (because SocialGist' API only allows for 32 query terms in a search)
        private const int maximumAllowedEntities = 32;

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            List<NewsEntity> userEntities = JsonUtility.Deserialize<List<NewsEntity>>(request.DataStore.GetValue("UserDefinedEntities"));

            try
            {
                if (userEntities != null)
                {
                    var sqlConn = request.DataStore.GetValue("SqlConnectionString");

                    var userEntityTable = UserEntitiesTable();

                    var count = 0;

                    foreach (NewsEntity userEntity in userEntities)
                    {
                        if (count == maximumAllowedEntities)
                        {
                            throw new Exception($"More than 32 aliases across all provided entities [{userEntities.Count}] were found; 32 is the maximum number allowed.");
                        }
                        var values = new List<string>(
                            userEntity.Values.Split(new string[] {"\n"},
                                System.StringSplitOptions.RemoveEmptyEntries));

                        foreach (var value in values)
                        {
                            var userEntityRow = userEntityTable.NewRow();
                            userEntityRow["regex"] = $"\\b{value}\\b"; // each term should be split on a word boundary
                            userEntityRow["entityValue"] = value;
                            userEntityRow["entityType"] = userEntity.Name;
                            userEntityRow["color"] = predefinedColors[count];
                            userEntityTable.Rows.Add(userEntityRow);
                            count++;
                        }
                    }
                    BulkInsert(sqlConn, userEntityTable, "reddit.UserDefinedEntityDefinitions");
                }

                return new ActionResponse(ActionStatus.Success);
            }
            catch (Exception e)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    e,
                    DefaultErrorCodes.DefaultErrorCode,
                    "An error occurred populating search entities"
                );
            }
        }


        public static void BulkInsert(string connString, DataTable table, string tableName)
        {
            try
            {
                using (var bulk = new SqlBulkCopy(connString))
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

        public static DataTable UserEntitiesTable()
        {
            var table = new DataTable();
            table.Columns.Add("regex");
            table.Columns.Add("entityType");
            table.Columns.Add("entityValue");
            table.Columns.Add("color");
            return table;
        }
    }
}