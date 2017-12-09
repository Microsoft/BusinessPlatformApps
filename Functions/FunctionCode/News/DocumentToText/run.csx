#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Text;
using Newtonsoft.Json;

using TikaOnDotNet.TextExtraction;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"DocumentConverter webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.document == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass document property in the input object"
        });
    }

    log.Info($"Input document with length {data.document.ToString().Length}");

    var input = Convert.FromBase64String(data.document.ToString());

    var resultObj = new TextExtractor().Extract(input);

    var result = resultObj.Text.Trim();

    log.Info($"DocumentConverter webhook finished with result '{result}'");

    return req.CreateResponse(HttpStatusCode.OK, new {
        text = result
    });

}
