using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Actions.Salesforce.Models;
using Microsoft.Deployment.Actions.Salesforce.SalesforceSOAP;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    public class SalesforceSqlArtefacts : BaseAction
    {
        List<string> coreObjects = new List<String>();

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string schema = "dbo";
            string connString = request.DataStore.GetValue("SqlConnectionString");
            coreObjects = request.DataStore.GetValue("ObjectTables").SplitByCommaSpaceTabReturnList();
            var objectMetadata = SalesforceUtility.GetMetadata(request).Result;

            List<Tuple<string, List<ADFField>>> adfFields = new List<Tuple<string, List<ADFField>>>();

            foreach (var obj in objectMetadata)
            {
                var simpleMetadata = ExtractSimpleMetadata(obj);

                adfFields.Add(new Tuple<string, List<ADFField>>(obj.name, simpleMetadata));

                CreateSqlTableAndTableType(simpleMetadata, obj.fields, schema, obj.name, connString);
                CreateIndexes(simpleMetadata, schema, obj.name.ToLower(), connString);
                CreateStoredProcedure(simpleMetadata, string.Concat("spMerge", obj.name), schema, string.Concat(obj.name.ToLowerInvariant(), "type"), obj.name.ToLowerInvariant(), connString);
            }

            request.DataStore.RemoveFirst("Objects");

            dynamic resp = new ExpandoObject();
            resp.ADFPipelineJsonData = new ExpandoObject();
            resp.ADFPipelineJsonData.fields = adfFields;

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromObject(resp));
        }

        public List<ADFField> ExtractSimpleMetadata(DescribeSObjectResult sfobject)
        {
            List<ADFField> simpleFields = new List<ADFField>();

            foreach (var field in sfobject.fields)
            {
                // check to go around ADF unsupported fields
                if (field.type != fieldType.address &&
                    field.type != fieldType.location &&
                    SupportedField(field))
                {
                    var newField = new ADFField();
                    var rawField = field.type;
                    newField.name = field.name.ToLowerInvariant();
                    var cleanedField = field.type.ToString().Contains('@') ? field.type.ToString().Replace('@', ' ') : field.type.ToString();
                    var netType = TypeMapper.SalesforceToDotNet.Where(p => p.Key == cleanedField).FirstOrDefault();
                    Debug.WriteLine(string.Concat(field.name, ", ", rawField, ", ", cleanedField, ", ", netType.Value));
                    if (netType.Key != null)
                    {
                        if (netType.Value == "int")
                        {
                            if (field.digits <= 5)
                            {
                                newField.type = "Int16";
                            }
                            if (field.digits > 5 && field.digits <= 10)
                            {
                                newField.type = "Int32";
                            }
                            if (field.digits > 10 && field.digits <= 19)
                            {
                                newField.type = "Int64";
                            }
                        }
                        else
                        {
                            newField.type = netType.Value;
                        }
                        simpleFields.Add(newField);
                    }
                }
            }
            return simpleFields;
        }

        public bool SupportedField(SalesforceSOAP.Field field)
        {
            List<string> userFieldsThatAreNotSupportedByADF = new List<string>();

            userFieldsThatAreNotSupportedByADF.Add("MediumPhotoUrl");
            userFieldsThatAreNotSupportedByADF.Add("UserPreferencesHideBiggerPhotoCallout");
            userFieldsThatAreNotSupportedByADF.Add("UserPreferencesHideSfxWelcomeMat");
            userFieldsThatAreNotSupportedByADF.Add("UserPreferencesHideLightningMigrationModal");
            userFieldsThatAreNotSupportedByADF.Add("UserPreferencesHideEndUserOnboardingAssistantModal");
            userFieldsThatAreNotSupportedByADF.Add("UserPreferencesPreviewLightning");

            if (userFieldsThatAreNotSupportedByADF.Contains(field.name))
            {
                return false;
            }
            return true;
        }

        private void CreateIndexes(List<ADFField> fields, string schemaName, string tableName, string connString)
        {
            string commandFormat = string.Empty;

            if (fields.Exists(f => f.name.ToLowerInvariant() == "isdeleted"))
            {
                commandFormat = $"CREATE INDEX idx_{tableName}_isDeleted ON [{schemaName}].[{tableName}] (isDeleted)";

                SqlUtility.InvokeSqlCommand(connString, commandFormat, null);
            }

            if (fields.Exists(f => f.name.ToLowerInvariant() == "lastmodifieddate"))
            {
                commandFormat = $"CREATE INDEX idx_lmd_{tableName} ON [{schemaName}].[{tableName}] (LastModifiedDate) include(id)";

                SqlUtility.InvokeSqlCommand(connString, commandFormat, null);
            }

        }

        public void CreateSqlTableAndTableType(List<ADFField> fields, SalesforceSOAP.Field[] sfFields, string schemaName, string tableName, string connString)
        {
            StringBuilder sbTable = new StringBuilder();
            StringBuilder sbTableType = new StringBuilder();

            string existingColumnsCmd = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND TABLE_SCHEMA = '{schemaName}'";

            var queryResult = SqlUtility.RunCommand(connString, existingColumnsCmd, SqlCommandType.ExecuteWithData);

            var existingColumns = from DataRow dr in queryResult.Rows
                                  select dr["COLUMN_NAME"];
            string createTable;

            if (coreObjects.Contains(tableName))
            {
                createTable = string.Format("ALTER TABLE [{0}] ADD", tableName.ToLowerInvariant());
            }
            else
            {
                createTable = string.Format("CREATE TABLE [{0}] (", tableName.ToLowerInvariant());
            }

            string createTableType = string.Format("CREATE TYPE [{0}].[{1}Type] AS TABLE(", schemaName, tableName.ToLowerInvariant());

            sbTable.AppendLine(createTable);
            sbTableType.AppendLine(createTableType);

            foreach (var field in fields)
            {
                if (!existingColumns.ToList().Contains(field.name))
                {
                    createTable = CreatePayload(sfFields, sbTable, field);
                }

                createTableType = CreatePayload(sfFields, sbTableType, field);
            }

            string tableTypeCmd = createTableType.Remove(createTableType.Length - 3, 1);
            tableTypeCmd = tableTypeCmd + ")";
            SqlUtility.InvokeSqlCommand(connString, tableTypeCmd, null);

            string createTableCmd = string.Empty;
            if (coreObjects.Contains(tableName))
            {
                createTableCmd = createTable + ($"CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ([Id])");
            }
            else
            {
                createTableCmd = createTable + ($"CONSTRAINT [PK_{tableName}] PRIMARY KEY CLUSTERED ([Id]))");
            }
            SqlUtility.InvokeSqlCommand(connString, createTableCmd, null);
        }

        private string CreatePayload(SalesforceSOAP.Field[] sfFields, StringBuilder sb, ADFField field)
        {
            var sqlType = TypeMapper.DotNetToSql.Where(p => p.Key == field.type).First();

            if (sqlType.Key != null && sqlType.Value == "nvarchar")
            {
                int nvarcharSize = sfFields.First(e => e.name.ToLowerInvariant() == field.name).length;

                string size = string.Empty;

                if (nvarcharSize > 255)
                {
                    size = "max";
                }
                if (nvarcharSize == 0)
                {
                    size = "255";
                }

                string commandFormat = string.Empty;

                if (field.name == "id")
                {
                    commandFormat = "[{0}] [{1}]({2}),";
                }
                else
                {
                    commandFormat = "[{0}] [{1}]({2}) NULL,";
                }

                sb.AppendLine(string.Format(commandFormat,
                    field.name,
                    string.IsNullOrEmpty(sqlType.Value) ? field.type.ToString() : sqlType.Value,
                    !string.IsNullOrEmpty(size) ? size : nvarcharSize.ToString()));
            }
            else
            {
                sb.AppendLine(string.Format("[{0}] [{1}] NULL,", field.name, string.IsNullOrEmpty(sqlType.Value) ? field.type.ToString() : sqlType.Value));
            }
            return sb.ToString();
        }

        public void CreateStoredProcedure(List<ADFField> fields, string sprocName, string schemaName, string tableTypeName, string targetTableName, string connString)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(string.Format("CREATE procedure [{0}].[{1}] @{2} [{0}].[{3}] READONLY as BEGIN", schemaName, sprocName, targetTableName, tableTypeName));
            sb.AppendLine(string.Format("MERGE [{0}].[{1}] AS TARGET \r\nUSING\r\n(SELECT", schemaName, targetTableName));

            foreach (var field in fields)
            {
                sb.AppendLine(string.Format("[{0}],", field.name));
            }

            sb.Remove(sb.Length - 3, 1);

            if (fields.Exists(f => f.name.ToLowerInvariant() == "lastmodifieddate"))
            {
                sb.AppendLine(string.Format("FROM @{0}\r\n) AS SOURCE\r\n ON SOURCE.ID = TARGET.ID \r\n WHEN MATCHED AND source.[LastModifiedDate] > target.[LastModifiedDate] THEN", targetTableName));
            }
            else
            {
                sb.AppendLine(string.Format("FROM @{0}\r\n) AS SOURCE\r\n ON SOURCE.ID = TARGET.ID \r\n WHEN MATCHED THEN", targetTableName));
            }

            sb.AppendLine("UPDATE \r\n SET");

            foreach (var field in fields)
            {
                sb.AppendLine(string.Format("TARGET.[{0}] = SOURCE.[{0}],", field.name));
            }

            sb.Remove(sb.Length - 3, 1);

            sb.AppendLine("WHEN NOT MATCHED BY TARGET THEN \r\nINSERT(");

            foreach (var field in fields)
            {
                sb.AppendLine(string.Format("[{0}],", field.name));
            }

            sb.Remove(sb.Length - 3, 1);
            sb.Append(")\r\n VALUES (");

            foreach (var field in fields)
            {
                sb.AppendLine(string.Format("SOURCE.[{0}],", field.name));
            }

            sb.Remove(sb.Length - 3, 1);
            sb.Append(");");

            var containsDelete = fields.Select(p => p.name == "IsDeleted");

            if (containsDelete.Contains(true))
            {
                sb.AppendLine($"DELETE FROM [{schemaName}].[{targetTableName}]");
                sb.AppendLine("WHERE IsDeleted = 1");
            }
            sb.AppendLine(@"END");

            SqlUtility.InvokeSqlCommand(connString, sb.ToString(), null);
        }
    }
}