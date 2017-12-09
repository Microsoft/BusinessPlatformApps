#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{

    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    var unescapedUri = Uri.UnescapeDataString((string)data.url);
    Regex r = new Regex("\\\\?.*&h=(.*)&v=.*&r=(.*)&p=");
    Match m = r.Match(unescapedUri);
    
    UriId returnObject = new UriId
    {
        newUri = m.Groups[2].Value,
        triggerId = m.Groups[1].Value
    };

    return req.CreateResponse(HttpStatusCode.OK, JsonConvert.SerializeObject(returnObject));
}

    public class UriId
    {
        public string newUri { get; set; }
        public string triggerId { get; set; }
    }
