#r "Newtonsoft.Json"
#r "System.Data"

using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;

public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
        using (var client = new HttpClient())
        {
            //Request headers
            string apiKey = System.Configuration.ConfigurationManager.ConnectionStrings["apiKey"].ConnectionString;
            string url = "https://api.cognitive.microsoft.com/bing/v5.0/news/search?q=microsoft";
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
            client.BaseAddress = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                log.Info("true");
                return req.CreateResponse("true");
            }
            else
            {
                log.Info("false");
                Thread.Sleep(30000);
                return req.CreateResponse("false");
            }
        }
}