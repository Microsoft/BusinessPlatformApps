#r "System.Data"
using System;
using System.Data;
using System.Data.SqlClient;

public static void Run(TimerInfo myTimer, TraceWriter log)
{
    string sqlConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
    string cognitiveKey = System.Configuration.ConfigurationManager.ConnectionStrings["CognitiveKey"].ConnectionString;
    string schema = System.Configuration.ConfigurationManager.ConnectionStrings["Schema"].ConnectionString;

    RunStoredProcWithRunId(sqlConnectionString, schema + ".UpdateEdges");
}

public static void RunStoredProcWithRunId(string sqlConnectionString, string storedProc)
{
    using (SqlConnection conn = new SqlConnection(sqlConnectionString))
    {
        using (
            SqlCommand command = new SqlCommand(storedProc, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
        {
            command.CommandTimeout = 180;
            conn.Open();
            var obj = command.ExecuteScalar();
            conn.Close();
        }
    }
}