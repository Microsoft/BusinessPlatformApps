using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Http
{
    public class HttpErrorResponse
    {
        [JsonProperty("code")]
        public string Code;
        [JsonProperty("message")]
        public string Message;
    }
}