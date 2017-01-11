#r "Newtonsoft.Json"

#load "..\Shared\StringUtilities.csx"

using System.Net;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"C# HTTP trigger function processed a request. RequestUri={req.RequestUri}");

    // parse query parameter
    string text = req.GetQueryNameValuePairs()
        .FirstOrDefault(q => string.Compare(q.Key, "text", true) == 0)
        .Value;

    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    text = text ?? data?.text;

    string asciiText = new StringUtilities().RemoveUnicode(text);

    return text == null
        ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a text parameter on the query string or in the request body")
        : req.CreateResponse(HttpStatusCode.OK, new { text = asciiText });
}
