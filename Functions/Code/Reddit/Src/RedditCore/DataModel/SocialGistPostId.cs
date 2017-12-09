using System;

namespace RedditCore.DataModel
{
    /// <summary>
    /// Contains all fields needed to provide to SocialGist to identify a particular post
    /// </summary>
    public class SocialGistPostId : IComparable
    {
        /// <summary>
        /// Gets or sets the URL of the post/comment
        /// </summary>
        public string Url { get; set; }

        public override string ToString()
        {
            return $"Url {Url}";
        }

        protected bool Equals(SocialGistPostId other)
        {
            return string.Equals(Url, other.Url);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SocialGistPostId) obj);
        }

        public override int GetHashCode()
        {
            return (Url != null ? Url.GetHashCode() : 0);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(null, obj)) return 1;
            if (obj.GetType() != this.GetType()) return 1;
            var other = (SocialGistPostId) obj;
            return string.Compare(Url, other.Url, StringComparison.Ordinal);
        }
    }
}
