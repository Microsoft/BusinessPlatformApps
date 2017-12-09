using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class CreateSqlUser : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            const string CMD_CREATE_USER = "CREATE LOGIN {0} WITH password='{1}'; " +
                                              "-- CREATE USER {0} FOR LOGIN {0} WITH DEFAULT_SCHEMA=[dbo];";
            const string CMD_ADD_USER_TO_DB = "CREATE USER {0} FOR LOGIN {0} WITH DEFAULT_SCHEMA=[dbo]; ";
            const string CMD_ADD_USER_TO_READER_ROLE = "EXEC sp_addrolemember 'db_datareader', '{0}'";
            const string CMD_ADD_USER_TO_WRITER_ROLE = "EXEC sp_addrolemember 'db_datawriter', '{0}'";
            const string CMD_GRANT_EXEC_TO_USER = "GRANT EXECUTE ON SCHEMA ::{1} TO {0}";

            JObject outputs = (JObject)JsonConvert.DeserializeObject(request.DataStore.GetValue("ArmOutput"));
            SqlConnectionStringBuilder cnBuilder = new SqlConnectionStringBuilder()
            {
                DataSource = outputs["sqlServerHostname"]["value"].ToString(),
                Encrypt = true,
                UserID = request.DataStore.GetValue("sqlServerAdminLogin"),
                Password = request.DataStore.GetValue("sqlServerAdminPassword"),
                InitialCatalog = "master"
            };

            string newUser = request.DataStore.GetValue("newUser");
            string newPassword = request.DataStore.GetValue("newPassword");
            using (SqlConnection cn = new SqlConnection(cnBuilder.ToString()))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand() { Connection = cn, CommandTimeout = 0 })
                {
                    cmd.CommandText = string.Format(CMD_CREATE_USER, newUser, newPassword);
                    cmd.ExecuteNonQuery();
                }
            }

            cnBuilder.InitialCatalog = request.DataStore.GetValue("databasename");
            // Add the final connection string to the datastore
            request.DataStore.RemoveFirst("SqlConnectionString");
            request.DataStore.AddToDataStore("SqlConnectionString", cnBuilder.ToString(), DataStoreType.Private);

            using (SqlConnection cn = new SqlConnection(request.DataStore.GetValue("SqlConnectionString")))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand() { Connection = cn, CommandTimeout = 0 })
                {
                    cmd.CommandText = string.Format(CMD_ADD_USER_TO_DB, newUser);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format(CMD_ADD_USER_TO_READER_ROLE, newUser);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format(CMD_ADD_USER_TO_WRITER_ROLE, newUser);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = string.Format(CMD_GRANT_EXEC_TO_USER, newUser, "dbo");
                    cmd.ExecuteNonQuery();

                    foreach (string s in request.DataStore.GetValue("additionalSchemas").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = string.Format(CMD_GRANT_EXEC_TO_USER, newUser, s.Trim());
                        cmd.ExecuteNonQuery();
                    }

                }
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}