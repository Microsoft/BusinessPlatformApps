#r "Newtonsoft.Json"

using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Collections.Generic;
using System.Text;


public static async Task<object> Run(HttpRequestMessage req, TraceWriter log)
{
            var client = new HttpClient();

            Document response = new Document()
            {
                Text = "test",
                Language = "en",
                Id = "1"
            };

            string subscriptionKey = System.Configuration.ConfigurationManager.ConnectionStrings["subscriptionKey"].ConnectionString;
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            var uri = "https://westus.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment";

            //Request body
            Sentiment sentiment = new Sentiment();
            sentiment.documents = new List<Document>();
            sentiment.documents.Add(response);

            string serializedSentiment = JsonConvert.SerializeObject(sentiment);
            StringContent content = new StringContent(serializedSentiment, Encoding.UTF8, "application/json");

            var sentimentResponse = await client.PostAsync(uri, content);
            
            if (sentimentResponse.IsSuccessStatusCode)
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

public class Document
{
    public string Language { get; set; }
    public string Id { get; set; }
    public string Text { get; set; }
}

public class Sentiment
{
    [JsonProperty(PropertyName = "documents")]
    public List<Document> documents { get; set; }
}
