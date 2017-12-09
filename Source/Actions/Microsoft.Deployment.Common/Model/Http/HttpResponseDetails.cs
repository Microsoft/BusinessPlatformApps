using System.Net;

namespace Microsoft.Deployment.Common.Model.Http
{
    public class HttpResponseDetails
    {
        public HttpStatusCode Code;
        public string Json;

        public HttpResponseDetails(HttpStatusCode code, string json)
        {
            Code = code;
            Json = json;
        }
    }
}