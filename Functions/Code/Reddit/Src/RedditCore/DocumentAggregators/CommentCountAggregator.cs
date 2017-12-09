using System.Collections.Generic;
using System.Linq;
using RedditCore.DataModel;
using RedditCore.DataModel.Repositories;

namespace RedditCore.DocumentAggregators
{
    internal class CommentCountAggregator : IDocumentAggregator
    {
        private readonly IRepository<PostCommentCount> repository;

        public CommentCountAggregator(IRepository<PostCommentCount> repository)
        {
            this.repository = repository;
        }

        public void Aggregate(IEnumerable<Post> posts, IEnumerable<Comment> comments)
        {
            var threadCommentCount = new Dictionary<string, int>();

            if (posts != null)
            {
                // Set all comment counts to zero incase a post exists without a comment
                foreach (var post in posts)
                {
                    threadCommentCount.Add(post.Id, 0);
                }
            }

            if (comments != null)
            {
                foreach (var comment in comments)
                {
                    if (!threadCommentCount.TryGetValue(comment.PostId, out var value))
                    {
                        value = 0;
                    }

                    threadCommentCount[comment.PostId] = value + 1;
                }
            }

            var results = threadCommentCount.Select(post =>
                new PostCommentCount()
                {
                    PostId = post.Key,
                    CommentCount = post.Value
                });

            this.repository.Save(results);
        }
    }
}
