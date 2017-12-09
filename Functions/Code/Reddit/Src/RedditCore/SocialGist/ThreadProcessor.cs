using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using RedditCore.DataModel;
using RedditCore.DataModel.Repositories;
using RedditCore.Logging;
using RedditCore.SocialGist.Model;
using RedditCore.Properties;
using RedditCore.DocumentFilters;
using RedditCore.DocumentAggregators;
using RedditCore.Telemetry;

namespace RedditCore.SocialGist
{
    internal class ThreadProcessor : IThreadProcessor
    {
        private readonly IRepository<Comment> commentRepository;
        private readonly ILog log;
        private readonly IObjectLogger objectLogger;
        private readonly IRepository<Post> postRepository;
        private readonly ISocialGist socialgist;

        private readonly Func<ResponsePost, Post> postFactory;
        private readonly Func<ResponseComment, Comment> commentFactory;

        private readonly IDocumentProcessor[] documentProcessors;
        private readonly IDocumentAggregator[] documentAggregators;
        private readonly IDocumentFilter[] documentFilters;

        private readonly ITelemetryClient telemetryClient;

        [Inject]
        public ThreadProcessor(
            IRepository<Post> postRepository,
            IRepository<Comment> commentRepository,
            ISocialGist socialgist,
            ILog log,
            IObjectLogger objectLogger,
            IDocumentProcessor[] documentProcessors,
            IDocumentFilter[] documentFilters,
            IDocumentAggregator[] documentAggregators,
            ITelemetryClient telemetryClient,
            Func<ResponsePost, Post> postFactory,
            Func<ResponseComment, Comment> commentFactory
        )
        {
            this.postRepository = postRepository;
            this.commentRepository = commentRepository;
            this.socialgist = socialgist;
            this.log = log;
            this.documentProcessors = documentProcessors;
            this.documentAggregators = documentAggregators;
            this.objectLogger = objectLogger;
            this.telemetryClient = telemetryClient;
            this.postFactory = postFactory;
            this.commentFactory = commentFactory;
            this.documentFilters = documentFilters;
        }

        public async Task Process(SocialGistPostId post)
        {
            this.objectLogger.Log(post, "ThreadProcessor");

            telemetryClient.TrackEvent(TelemetryNames.ThreadProcessor_ProcessThread);

            var result = await socialgist.CommentsForThread(post);
            var filteredPosts = new List<ResponsePost>() {result.Post}.Where(x =>
                !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Author) && x.CreatedUtc != 0);
            var filteredComments = result.Comments.Where(x =>
                !string.IsNullOrEmpty(x.Id) && !string.IsNullOrEmpty(x.Author) && x.CreatedUtc != 0);

            if (filteredComments.Any() || filteredPosts.Any())
            {
                IEnumerable<IDocument> allDocuments = FilterAndWriteDocuments(filteredPosts.ToList().AsReadOnly(), filteredComments.ToList().AsReadOnly()).ToList();

                // Perform all actions that need to be run over every post and comment
                foreach (var processor in this.documentProcessors)
                {
                    processor.ProcessDocuments(allDocuments);
                }
            }
            else
            {
                log.Info($"No thread found with url {post.Url}");
            }
        }

        private IEnumerable<IDocument> FilterAndWriteDocuments(ICollection<ResponsePost> allPosts, ICollection<ResponseComment> allComments)
        {
            // Create posts and comments.  Be sure to materialize into lists so we do not re-create items multiple times
            IEnumerable<Post> posts = allPosts.Select(x => postFactory(x)).ToList();
            IEnumerable<Comment> comments = allComments.Select(x => commentFactory(x)).ToList();

            var unfilteredPostCount = posts.Count();
            var unfilteredCommentCount = comments.Count();

            foreach(var aggregator in this.documentAggregators)
            {
                aggregator.Aggregate(posts, comments);
            }

            // Filter posts and comments if needed.  If no filters are defined then this loop will not happen.
            // Ninject always injects an empty array for multi-bindings if no objects are bound.
            foreach (var filter in this.documentFilters)
            {
                posts = posts.Where(filter.ShouldKeep).ToList();
                comments = comments.Where(filter.ShouldKeep).ToList();
            }

            var filteredPostCount = posts.Count();
            var filteredCommentCount = comments.Count();

            this.telemetryClient.TrackEvent(
                TelemetryNames.ThreadProcessor_FilteredCommentsAndPostsEvent,
                null,
                new Dictionary<string, double>
                {
                    { TelemetryNames.ThreadProcessor_UnfilteredPosts, unfilteredPostCount },
                    { TelemetryNames.ThreadProcessor_UnfilteredComments, unfilteredCommentCount },
                    { TelemetryNames.ThreadProcessor_FilteredPostCount, filteredPostCount },
                    { TelemetryNames.ThreadProcessor_FilteredCommentCount, filteredCommentCount }
                }
                );

            // Save posts
            postRepository.Save(posts);

            // Save comments
            this.log.Verbose($"Writing {comments.Count()} comments");
            commentRepository.Save(comments);
            this.log.Verbose($"Finished writing comments");

            // Return filtered list of all document types 
            return comments.Cast<IDocument>().Union(posts.Cast<IDocument>());
        }
    }
}
