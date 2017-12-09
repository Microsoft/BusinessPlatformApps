using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using Ninject;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class CommentRepository : RepositoryBase<Comment>
    {
        private readonly IDocumentRemover documentRemover;

        [Inject]
        public CommentRepository(EntityConnection connection, ILog log, IDocumentRemover documentRemover)
            : base(connection, log, "comment")
        {
            this.documentRemover = documentRemover;
        }

        protected override IEnumerable<Comment> PreSave(IEnumerable<Comment> items, RedditEntities db)
        {
            // Delete all comments.  Even the invalid comments.
            // A comment may be made on Reddit, ingested by us, then deleted.
            // On the next poll this comment will be seen as an InvalidComment.  We want
            // to remove it from the DB along with all the other comments.
            this.documentRemover.RemoveDocuments(items.ToList<IDocument>());

            // Do not write the invalid comments.
            return items.Where(x => !(x is InvalidComment));
        }

        protected override DbSet<Comment> GetDbSet(RedditEntities db)
        {
            return db.Comments;
        }
    }
}