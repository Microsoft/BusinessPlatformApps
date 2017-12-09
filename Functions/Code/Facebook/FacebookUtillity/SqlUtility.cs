using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace FacebookETL
{
    public class SqlUtility
    {
        public static string SanitizeSchemaName(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            Regex invalidCharacters = new Regex("[^a-zA-Z_0-9]");
            return invalidCharacters.Replace(value, string.Empty);
        }

        public static void RunStoredProc(string sqlConnectionString, string storedProc)
        {
            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(storedProc, conn) { CommandType = CommandType.StoredProcedure, CommandTimeout = 180 })
                {
                    command.ExecuteScalar();
                }
            }
        }

        public static void BulkInsert(string connString, DataTable table, string tableName)
        {
            try
            {
                using (SqlBulkCopy bulk = new SqlBulkCopy(connString))
                {
                    bulk.BatchSize = 5000;
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

        public static string[] GetPages(string sqlConnectionString, string schema)
        {
            string pagesCommaSeparated = null;
            string SQL_PAGES_TO_FOLLOW = $"SELECT [value] FROM {SanitizeSchemaName(schema)}.[configuration] WHERE [configuration_group] = 'SolutionTemplate' AND [configuration_subgroup] = 'ETL' AND [name] = 'PagesToFollow'";

            using (SqlConnection conn = new SqlConnection(sqlConnectionString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(SQL_PAGES_TO_FOLLOW, conn))
                {
                    SqlDataReader dr = command.ExecuteReader();
                    if (dr.Read())
                    {
                        if (dr[0] != null && dr[0] != DBNull.Value)
                            pagesCommaSeparated = dr[0].ToString();
                    }
                    dr.Close();
                }
            }

            return pagesCommaSeparated != null ? pagesCommaSeparated.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };
        }
    }
}
