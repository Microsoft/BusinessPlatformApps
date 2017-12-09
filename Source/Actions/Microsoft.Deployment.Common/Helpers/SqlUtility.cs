using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text.RegularExpressions;

using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Model;

namespace Microsoft.Deployment.Common.Helpers
{
    public class SqlUtility
    {
        private const int MAX_RETRIES = 10;
        private const string writeableDatabaseQuery = "CREATE TABLE {0}(pbi INT); INSERT INTO {0}(pbi) VALUES(1); DROP TABLE {0};";

        public static string SanitizeSchemaName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            Regex invalidCharacters = new Regex("[^a-zA-Z_0-9]");
            return invalidCharacters.Replace(value, string.Empty);

        }

        public static SqlParameter[] MapValuesToSqlParameters(params object[] list)
        {
            if (list == null)
                return null;

            SqlParameter[] result = new SqlParameter[list.Length];
            for (int i=0; i<list.Length; i++)
            {
                result[i] = new SqlParameter();
                if (list[i] is int)
                    result[i].DbType = DbType.Int32;
                else if (list[i] is uint)
                    result[i].DbType = DbType.UInt32;
                else if (list[i] is long)
                    result[i].DbType = DbType.Int64;
                else if (list[i] is ulong)
                    result[i].DbType = DbType.UInt64;
                else if (list[i] is short)
                    result[i].DbType = DbType.Int16;
                else if (list[i] is ushort)
                    result[i].DbType = DbType.UInt16;
                else if (list[i] is byte)
                    result[i].DbType = DbType.Byte;
                else if (list[i] is float)                     // Floating types
                    result[i].DbType = DbType.Single;
                else if (list[i] is double)
                    result[i].DbType = DbType.Double;
                else if (list[i] is DateTime)                  // Date and time
                    result[i].DbType = DbType.DateTime;
                else if (list[i] is bool)                      // Boolean
                    result[i].DbType = DbType.Boolean;
                else if (list[i] is char || list[i] is string) // Character type
                    result[i].DbType = DbType.String;
                else
                    throw new Exception("Unexpected data type"); // OUR code should not use other types
                
                result[i].Value = list[i];
                result[i].ParameterName = $"@p{i+1}"; // SqlClient doesn't accept anonymous parameters (expecting queries to use 1 based numbering for parameter names: @p1, @p2, etc...)
            }


            return result;
        }

        public static void ExecuteQueryWithParameters(string connectionString,  string command, SqlParameter[] parameters)
        {
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(command, cn) { CommandType = CommandType.Text, CommandTimeout = 0 })
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static List<string> GetListOfDatabases(string connectionString)
        {
            var result = new List<string>();

            if (string.IsNullOrEmpty(connectionString))
                return result;

            for (int retries = 0; retries < MAX_RETRIES; retries++)
            {
                SqlConnection cn = new SqlConnection(connectionString);
                try
                {
                    cn.Open();
                    DataTable databasesTable = cn.GetSchema("Databases");
                    foreach (DataRow row in databasesTable.Rows)
                        result.Add((string)row["database_name"]);

                    break;
                }
                catch (SqlException e)
                {
                    if ((cn.State == ConnectionState.Open) || (retries == MAX_RETRIES - 1) || (e.Number == 40615 && e.Class == 14))
                    {
                        throw;
                    }

                    if (cn.State != ConnectionState.Open)
                    {
                        // There was a problem with this connection that might not be fatal, let's retry
                    }
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }

        private static bool IsDatabaseWriteEnabled(string connectionString)
        {
            bool result = false;

            if (string.IsNullOrEmpty(connectionString))
                return false;

            for (int retries = 0; retries < MAX_RETRIES; retries++)
            {
                SqlConnection cn = new SqlConnection(connectionString);
                try
                {
                    cn.Open();
                    string tableName = "PBI" + Guid.NewGuid().ToString("N");
                    using (SqlCommand cmd = new SqlCommand(string.Format(CultureInfo.InvariantCulture, SqlUtility.writeableDatabaseQuery, tableName), cn))
                    {
                        cmd.CommandTimeout = 0;
                        cmd.ExecuteNonQuery();
                        result = true; // Won't reach this point if we don't have write permissions
                    }

                    break;
                }
                catch (Exception)
                {

                    if (cn.State == ConnectionState.Open)
                    {
                        // The connection is good, likely the database is not writeable, let's break the loop
                        break;
                    }

                    // Since we didn't break above, there was a problem with the connection and might be transient, let's retry
                }
                finally
                {
                    cn.Close();
                }
            }

            return result;
        }

        public static List<string> GetListOfDatabases(SqlCredentials credentials, bool showOnlyWriteEnabled = false, bool showSystemDB = false)
        {
            var connectionString = GetConnectionString(credentials);

            var result = GetListOfDatabases(connectionString);

            if (showOnlyWriteEnabled)
            {
                var databasesToReturn = new List<string>();
                foreach (var database in result)
                {
                    credentials.Database = database;
                    connectionString = GetConnectionString(credentials);

                    if (IsDatabaseWriteEnabled(connectionString))
                        databasesToReturn.Add(database);
                }

                result = databasesToReturn;
            }

            if (!showSystemDB)
            {
                result.RemoveAll(p =>
                    string.Equals("master", p, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("tempdb", p, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("msdb", p, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals("model", p, StringComparison.OrdinalIgnoreCase));
            }

            return result;
        }

        public static void InvokeSqlCommand(string connectionString, string script, Dictionary<string, string> args)
        {
            string[] batches = ParseSql(script);
            for (int i = 0; i < batches.Length; i++)
            {
                batches[i] = ReplaceScriptWithArgs(batches[i], args);
                RunCommand(connectionString, batches[i], SqlCommandType.ExecuteWithoutData);
            }
        }

        public static DataTable InvokeStoredProcedure(SqlCredentials credentials, string script, Dictionary<string, string> args)
        {
            return InvokeStoredProcedure(GetConnectionString(credentials), script, args);
        }

        public static DataTable InvokeStoredProcedure(string ConnectionString, string script, Dictionary<string, string> args)
        {
            script = ReplaceScriptWithArgs(script, args);
            return RunCommand(ConnectionString, script, SqlCommandType.ExecuteStoredProc);
        }

        public static DataTable InvokeSqlCommandWithData(SqlCredentials credentials, string script, Dictionary<string, string> args)
        {
            var connectionString = GetConnectionString(credentials);
            script = ReplaceScriptWithArgs(script, args);
            return RunCommand(connectionString, script, SqlCommandType.ExecuteWithData);
        }

        public static DataTable RunCommand(string connectionString, string rawScript, SqlCommandType commandType, SqlParameter[] parameters = null)
        {
            DataTable table = null;

            if (string.IsNullOrWhiteSpace(rawScript))
                return null;

            for (var retries = 0; retries < MAX_RETRIES; retries++)
            {
                SqlTransaction transaction = null;
                var cn = new SqlConnection(connectionString);

                try
                {
                    cn.Open();
                    transaction = cn.BeginTransaction(IsolationLevel.ReadCommitted);
                    using (var command = cn.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = rawScript;
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = 0;

                        if (parameters != null)
                            command.Parameters.AddRange(parameters);

                        switch (commandType)
                        {
                            case SqlCommandType.ExecuteWithData:
                                {
                                    table = new DataTable();
                                    var adapter = new SqlDataAdapter(command);
                                    adapter.Fill(table);
                                    break;
                                }
                            case SqlCommandType.ExecuteStoredProc:
                                {
                                    command.CommandType = CommandType.StoredProcedure;
                                    table = new DataTable();
                                    var adapter = new SqlDataAdapter(command);
                                    adapter.Fill(table);
                                    break;
                                }
                            case SqlCommandType.ExecuteWithoutData:
                                {
                                    command.ExecuteNonQuery();
                                    break;
                                }
                        }
                    }

                    transaction.Commit();
                    break;
                }
                catch (Exception)
                {
                    if (cn.State != ConnectionState.Open)
                    {
                        // The transaction must have been rolledback with the client being disconnected
                        try
                        {
                            transaction?.Rollback();
                        }
                        catch (Exception)
                        {
                        }

                        continue;
                    }

                    throw;
                }
                finally
                {
                    cn.Close();
                }
            }

            return table;
        }

        private static string[] ParseSql(string script)
        {
            return Regex.Split(script, @"\s*go\s*\r\n", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
        }

        public static string ReplaceScriptWithArgs(string script, Dictionary<string, string> args)
        {
            if (args == null)
            {
                return script;
            }

            string result = script;
            foreach (string k in args.Keys)
            {
                result = result.Replace(k, args[k]);
            }

            return result;
        }

        public static string GetConnectionString(SqlCredentials credentials)
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder
            {
                DataSource = credentials.Server,
                ConnectTimeout = 15
            };

            // Add encryption if we're targeting an Azure server
            if (credentials.Server.IndexOf(".database.windows.net", StringComparison.OrdinalIgnoreCase) > 0)
            {
                conn.Encrypt = true;
                conn.TrustServerCertificate = false;
            }

            conn.InitialCatalog = credentials.Database ?? (credentials.AlternativeDatabaseToConnect ?? "master");

            if (credentials.Authentication == SqlAuthentication.SQL)
            {
                conn.IntegratedSecurity = false;
                conn.UserID = credentials.Username;
                conn.Password = credentials.Password;
            }
            else
                conn.IntegratedSecurity = true;

            return conn.ConnectionString;
        }

        public static string GetPythonConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder(connectionString);
            return $"SERVER={conn.DataSource};DATABASE={conn.InitialCatalog};PWD={conn.Password};UID={conn.UserID}";

        }

        public static SqlCredentials GetSqlCredentialsFromConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder conn = new SqlConnectionStringBuilder(connectionString);
            return new SqlCredentials()
            {
                Database = conn.InitialCatalog,
                Server = conn.DataSource,
                Username = conn.UserID,
                Authentication = conn.IntegratedSecurity ? SqlAuthentication.Windows : SqlAuthentication.SQL,
                Password = conn.Password
            };
        }
    }
}