using System.Net;
using System;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    // Get request body
    dynamic data = await req.Content.ReadAsAsync<object>();

    // Set name to query string or body data
    int days = data?.value;

     if(days > 0)
        {
            days = days * -1;
        }

        string dateToReturn =  DateTime.Now.AddDays(days).ToString();

    return req.CreateResponse(HttpStatusCode.OK, dateToReturn);
}
