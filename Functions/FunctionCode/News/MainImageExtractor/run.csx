#r "Newtonsoft.Json"

#load "ImageResult.csx"

using System;
using System.Net;
using Newtonsoft.Json;
using NSoup;
using NSoup.Nodes;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.html == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass html property in the input object"
        });
    }

    var html = data.html.ToString();
    log.Verbose($"Input data: {html}");

    var doc = NSoupClient.Parse(html);

    var image = doc.Select("[property=og:image]").Attr("content");

    if (String.IsNullOrEmpty(image))
    {
        image = doc.Select("[name=twitter:image]").Attr("content");
    }
    else
    {
        log.Verbose($"Found image {image} in OpenGraph annotation");
    }

    if (String.IsNullOrEmpty(image))
    {
        log.Verbose($"No image found");
        image = null;
    }
    else
    {
        log.Verbose($"Returning image {image}");
    }

    var result = new ImageResult() { ImageUrl = image };

    return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(result));
}
