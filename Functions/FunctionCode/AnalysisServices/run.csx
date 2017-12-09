
#r "Microsoft.AnalysisServices.dll"
#r "Microsoft.AnalysisServices.Core.dll"
#r "Microsoft.AnalysisServices.Tabular.dll"
#r "Microsoft.IdentityModel.Clients.ActiveDirectory.Platform.dll"
#r "Microsoft.IdentityModel.Clients.ActiveDirectory.dll"
    
#r "System.Data"

using System.Data;
using System.Data.SqlClient;
using System.Net;
using RefreshType = Microsoft.AnalysisServices.Tabular.RefreshType;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Server = Microsoft.AnalysisServices.Tabular.Server;

public static async Task Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");
    // Configuration
    string appId = System.Configuration.ConfigurationManager.ConnectionStrings["appId"].ConnectionString;
    string appKey = System.Configuration.ConfigurationManager.ConnectionStrings["appKey"].ConnectionString;
    string username = System.Configuration.ConfigurationManager.ConnectionStrings["username"].ConnectionString;
    string tenantId = System.Configuration.ConfigurationManager.ConnectionStrings["tenantId"].ConnectionString;
    string asServer = System.Configuration.ConfigurationManager.ConnectionStrings["asServer"].ConnectionString;

    string databaseAS = System.Configuration.ConfigurationManager.ConnectionStrings["databaseAS"].ConnectionString;
    string connStringSql = System.Configuration.ConfigurationManager.ConnectionStrings["connStringSql"].ConnectionString;
    string schema = System.Configuration.ConfigurationManager.ConnectionStrings["schema"].ConnectionString;
    string functionName = System.Configuration.ConfigurationManager.ConnectionStrings["functionName"].ConnectionString;

    Uri asServerUrl = new Uri(asServer);
    string resource = "https://" + asServerUrl.Host;
    AuthenticationContext context = new AuthenticationContext("https://login.windows.net/" + tenantId);
    ClientCredential credential = new ClientCredential(appId, appKey);
    var token = await context.AcquireTokenAsync(resource, credential);

    string password = token.AccessToken;
    string connectionString = $"Provider=MSOLAP;Data Source={asServer};Password={password};";

    string responseSuccess = "Success";
    string id = "";
    try
    {
        string functionSql = ExecuteSql(connStringSql,
            $"SELECT [value] FROM {schema}.[configuration] WHERE[configuration_group] = 'SolutionTemplate' AND[configuration_subgroup] = 'SSAS' AND[name] = 'FunctionName';");
        
        if (functionSql != functionName)
        {
            log.Info("function name didnt match");
            return;
        }

        id = ExecuteStoredProcedure(connStringSql, schema, "[sp_start_job]");
        if (string.IsNullOrEmpty(id) || id == "0")
        {
            return;
        }

         string lastDateProcessed = ExecuteSql(connStringSql,
            $"SELECT TOP 1 [endTime] FROM {schema}.[ssas_jobs] WHERE [statusMessage] = 'Success' ORDER BY [endTime] DESC;");

        bool process = true;
        if(lastDateProcessed != null)
        {
            DateTime lastProcessed = DateTime.Parse(lastDateProcessed);
            DateTime now = DateTime.Now;
            if(now - lastProcessed < TimeSpan.FromMinutes(15))
            {
                process = false;
            }
        }    
       
        if(process)
        {
            log.Info("Trying to connect");
            Server server = new Server();
            server.Connect(connectionString);
            log.Info("Connected");
            var db = server.Databases.Find(databaseAS);
            log.Info("found db");
            db.Model.RequestRefresh(RefreshType.Full);
            log.Info("Request process");
            db.Model.SaveChanges();
            log.Info("Processed");
        }
        else
        {
            responseSuccess = "Last processed time was less than 15 miniutes ago";
        }
    }
    catch (Exception ex)
    {
        log.Info("Exception");
        responseSuccess = ex.Message;
    }

    log.Info("Write result back to DB");
    Dictionary<string, string> param = new Dictionary<string, string>();
    param.Add("@jobid", id);
    param.Add("@jobMessage", responseSuccess);
    ExecuteStoredProcedure(connStringSql, schema, "[sp_finish_job]", param);
    log.Info("write back to db completed");
    log.Info("Finished");
}

public static string ExecuteStoredProcedure(string connectionString, string schema, string spName)
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        using (
            SqlCommand command = new SqlCommand(schema + "." + spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
        {
            conn.Open();
            SqlParameter retval = command.Parameters.Add("@returnval", SqlDbType.VarChar);
            retval.Direction = ParameterDirection.ReturnValue;
            command.ExecuteNonQuery();
            return command.Parameters["@returnval"].Value.ToString();

        }
    }
}

public static string ExecuteSql(string connectionString, string sqlStatement)
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        using (
            SqlCommand command = new SqlCommand(sqlStatement, conn)
            {
                CommandType = CommandType.Text
            })
        {
            conn.Open();
            var data = command.ExecuteReader();
            if(data.HasRows)
            {
                data.Read();
                return data[0].ToString();
            }
            
            return null;
        }
    }
}

public static string ExecuteStoredProcedure(string connectionString, string schema, string spName,
    Dictionary<string, string> param)
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        using (
            SqlCommand command = new SqlCommand(schema + "." + spName, conn)
            {
                CommandType = CommandType.StoredProcedure
            })
        {
            conn.Open();
            foreach (var keyValuePair in param)
            {
                command.Parameters.Add(new SqlParameter(keyValuePair.Key, keyValuePair.Value));
            }

            SqlParameter retval = command.Parameters.Add("@returnval", SqlDbType.VarChar);
            retval.Direction = ParameterDirection.ReturnValue;
            command.ExecuteNonQuery();
            return command.Parameters["@returnval"].Value.ToString();
        }
    }
}
