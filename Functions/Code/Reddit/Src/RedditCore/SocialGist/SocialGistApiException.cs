using System;

namespace RedditCore.SocialGist
{
    public class SocialGistApiException : Exception
    {
        public SocialGistApiException()
            : base()
        {

        }

        public SocialGistApiException(string message)
            : base(message)
        {

        }
    }
}
