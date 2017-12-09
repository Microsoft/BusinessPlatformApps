using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Http
{
    public class HttpErrorResponseWrapper
    {
        [JsonProperty("error")]
        public HttpErrorResponse Error;
    }
}