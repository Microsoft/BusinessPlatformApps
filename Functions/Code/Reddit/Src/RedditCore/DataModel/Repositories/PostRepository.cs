using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class PostRepository : RepositoryBase<Post>
    {
        private readonly IDocumentRemover documentRemover;

        [Inject]
        public PostRepository(EntityConnection connection, ILog log, IDocumentRemover documentRemover)
            : base(connection, log, "post")
        {
            this.documentRemover = documentRemover;
        }

        protected override IEnumerable<Post> PreSave(IEnumerable<Post> items, RedditEntities db)
        {
            documentRemover.RemoveDocuments(items.ToList<IDocument>());

            // No need to filter posts.
            return items;
        }

        protected override DbSet<Post> GetDbSet(RedditEntities db)
        {
            return db.Posts;
        }
    }
}
