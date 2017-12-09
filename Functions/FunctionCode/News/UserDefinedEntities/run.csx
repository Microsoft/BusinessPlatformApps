#r "Newtonsoft.Json"

#load "EntityResult.csx"
#load "EntityDefinition.csx"
#load "EntityDefinitionReader.csx"

using System;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

/*
Configuration:
NOTE: Regular expression parser is configured to be case-insensitive
* Add a new Connection String to the web app called "connectionString".  This should point to the bingnews SQL database.
* Add a new row to the "userdefinedentitydefinitions" table for each regex you want to search with
** "regex":"Google", "entityType":"Company", "entityValue": "Google, Inc."
** "regex":"Microsoft", "entityType":"Company", "entityValue": "Microsoft, Inc."
** etc

Input:
{
    text: "My Document Here"
}

Output:
{
    "entities":[
        {"value":"Google, Inc","type":"Company","position":0,"lengthInText":6}
        , etc
    ]
}
*/

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    string connectionString;

    try
    {
        connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
    }
    catch (Exception e)
    {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Error retrieving appsetting 'connectionString'.  Please ensure that the setting is defined. " + e.ToString()});
    }

    if (data.text == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass text property in the input object"
        });
    }

    var resultList = new LinkedList<EntityResult>();

    var textLength = (double) data.text.ToString().Length;

	var entityDefs = new EntityDefinitionReader(connectionString).LoadEntityDefinitions();

    foreach (EntityDefinition byoEntity in entityDefs)
    {
        var regex = new Regex(byoEntity.Regex, RegexOptions.IgnoreCase);

        foreach( Match match in regex.Matches(data.text.ToString()))
        {
            var entity = new EntityResult() {
                value = byoEntity.EntityValue,
                type = byoEntity.EntityType,
                position = match.Index,
                positionDocumentPercentage = Math.Max((double) match.Index / textLength, 0.000001),
                lengthInText = match.Value.Length
            };

            resultList.AddLast(entity);

            log.Verbose($"Found {match.Value} at position {match.Index}");
        }
    }

    return req.CreateResponse(HttpStatusCode.OK, new {
        entities = resultList
    });
}
