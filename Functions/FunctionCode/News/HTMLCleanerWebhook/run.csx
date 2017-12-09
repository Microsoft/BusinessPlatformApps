#r "Newtonsoft.Json"

#load "..\shared\StringExtensions.csx"
#load "..\shared\HtmlCleaner.csx"

using System;
using System.Net;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    if (data.html == null) {
        return req.CreateResponse(HttpStatusCode.BadRequest, new {
            error = "Please pass 'html' property in the input object"
        });
    }

    var result = new HtmlCleaner().Clean(data.html.ToString());

    String scrubbed = result.Scrubbed;

    var scrubbedAndTrimmed = StringExtensions.LimitByBytes(scrubbed, 10000);

    return req.CreateResponse(HttpStatusCode.OK, new {
        Scrubbed = scrubbedAndTrimmed,
        ScrubbedLength = scrubbedAndTrimmed.Length,
        NoTags = result.NoTags,
        NoTagsLength = result.NoTags.Length,
        TopImage = result.Image
    });
}
