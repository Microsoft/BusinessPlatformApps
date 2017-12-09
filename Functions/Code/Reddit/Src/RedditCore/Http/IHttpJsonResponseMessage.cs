using System.Net.Http;

namespace RedditCore.Http
{
    public interface IHttpJsonResponseMessage<T>
    {
        T Object { get; }

        HttpResponseMessage ResponseMessage { get; }
    }
}
