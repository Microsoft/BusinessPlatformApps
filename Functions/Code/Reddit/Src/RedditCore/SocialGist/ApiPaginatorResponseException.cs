using System;

namespace RedditCore.SocialGist
{
    public class ApiPaginatorResponseException : Exception
    {
        public ApiPaginatorResponseException()
            : base()
        {

        }

        public ApiPaginatorResponseException(string message)
            : base(message)
        {

        }
    }
}
