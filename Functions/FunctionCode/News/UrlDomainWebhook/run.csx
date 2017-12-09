#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.url == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please add a 'url' property with a valid URI"
        });
    }

    var myUri = new Uri(data.url.ToString());

    return req.CreateResponse(HttpStatusCode.OK, new {
        host = myUri.Host
    });
}
