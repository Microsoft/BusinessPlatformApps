#r "Newtonsoft.Json"

#load "DocumentTimeFactory.csx"
#load "DocumentTime.csx"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.date == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass date property in the input object"
        });
    }

    DateTime input = data.date;

    log.Info($"DateTime: {input}");

    var factory = new DocumentTimeFactory();

    DocumentTime result = factory.CreateDocumentTime(input);

    return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
}
