using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using RedditCore.Logging;

namespace RedditCore.DataModel.Repositories
{
    internal class PostCommentCountRepository : RepositoryBase<PostCommentCount>
    {
        public PostCommentCountRepository(EntityConnection connection, ILog log)
            : base(connection, log, "PostCommentCount")
        {
        }

        protected override IEnumerable<PostCommentCount> PreSave(IEnumerable<PostCommentCount> items, RedditEntities db)
        {
            var ids = items.Select(x => x.PostId);
            var existingItems = (from postCommentCount in db.PostCommentCounts
                                 where ids.Contains(postCommentCount.PostId)
                                 select postCommentCount).ToList();
            db.PostCommentCounts.RemoveRange(existingItems);
            db.SaveChanges();

            return items;
        }

        protected override DbSet<PostCommentCount> GetDbSet(RedditEntities db)
        {
            return db.PostCommentCounts;
        }
    }
}
