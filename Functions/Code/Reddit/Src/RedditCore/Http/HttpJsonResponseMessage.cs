using System.Net.Http;

namespace RedditCore.Http
{
    public class HttpJsonResponseMessage<T> : IHttpJsonResponseMessage<T>
    {
        public T Object { get; internal set; }

        public HttpResponseMessage ResponseMessage { get; internal set; }
    }
}
