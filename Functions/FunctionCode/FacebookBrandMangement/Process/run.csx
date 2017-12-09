#r "FacebookUtillity.dll"

using System;
using FacebookUtillity;

public static async Task Run(string myQueueItem, TraceWriter log)
{
    log.Info($"C# Queue trigger function processed: {myQueueItem}");

    string sqlConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlConnectionString"].ConnectionString;
    string cognitiveKey = System.Configuration.ConfigurationManager.ConnectionStrings["CognitiveKey"].ConnectionString;
    string schema = System.Configuration.ConfigurationManager.ConnectionStrings["Schema"].ConnectionString;
    string facebookClientId = System.Configuration.ConfigurationManager.ConnectionStrings["FacebookClientId"].ConnectionString;
    string facebookClientSecret = System.Configuration.ConfigurationManager.ConnectionStrings["FacebookClientSecret"].ConnectionString;
    string date = myQueueItem;

    await MainETL.PopulateAll(sqlConnectionString, schema, cognitiveKey, facebookClientId, facebookClientSecret, date);
}
