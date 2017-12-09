using RedditCore.DataModel;
using System.Collections.Generic;

namespace RedditCore.DocumentAggregators
{
    public interface IDocumentAggregator
    {
        void Aggregate(IEnumerable<Post> posts, IEnumerable<Comment> comments);
    }
}
