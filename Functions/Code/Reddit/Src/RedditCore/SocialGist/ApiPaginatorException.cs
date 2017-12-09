using System;

namespace RedditCore.SocialGist
{
    public class ApiPaginatorException : Exception
    {
        public ApiPaginatorException()
            : base()
        {

        }

        public ApiPaginatorException(string message)
            : base(message)
        {

        }
    }
}
