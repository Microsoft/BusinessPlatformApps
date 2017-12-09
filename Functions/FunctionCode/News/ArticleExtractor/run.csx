#r "Newtonsoft.Json"
#r "Microsoft.KnowledgeMining.MainArticleExtractor.dll"

using System;
using System.Net;
using Newtonsoft.Json;

using Microsoft.KnowledgeMining.MainArticleExtractor.Extractors;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Article Extractor was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);
    string content = data.html;
    string url = data.url;

    if (content == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "'html' property missing from the input JSON"
        });
    }

    MainBlockExtractor x = new MainBlockExtractor();
    var result = x.Extract(content, url);

    return req.CreateResponse(HttpStatusCode.OK, new {
        bodyInHtml = result.BodyInHtml
    });
}
