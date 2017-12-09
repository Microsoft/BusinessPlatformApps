#r "Newtonsoft.Json"

using System;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info($"Webhook was triggered!");

    string jsonContent = await req.Content.ReadAsStringAsync();
    dynamic data = JsonConvert.DeserializeObject(jsonContent);

    log.Info(data.ToString());

    int previousCount;
    int currentCount; 

    if(data.PreviousCount == "")
    {
        previousCount = 0;
    }
    else
    {
        previousCount = data.PreviousCount;
    }

    if(data.CurrentCount == "")
    {
        currentCount = 0;
    }
    else
    {
        currentCount = data.CurrentCount;
    }

    int delay = data.Delay;
    int delta = currentCount - previousCount;



    if (currentCount < 40)
    {
        delay = delay + 3;
    }
    else if (currentCount > 50)
    {
        if (delta > 30)
        {
            delay = 1;
        }
        else
        {
            delay = delay - 3;
        }
    }


    if (delay < 1)
    {
        delay = 1;
    }
    else if (delay > 30)
    {
        delay = 30;
    }


    string requestUri = System.Configuration.ConfigurationManager.ConnectionStrings["requestUri"].ConnectionString;
    string body = "{\"PreviousCount\":" + currentCount + ", \"Delay\":" + delay + "}";

    using (HttpClient client = new HttpClient())
    {
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, requestUri);
        message.Content = new StringContent(body, Encoding.UTF8, "application/json");
        var response = await client.SendAsync(message);
    }

        return req.CreateResponse(HttpStatusCode.OK, "");
}
