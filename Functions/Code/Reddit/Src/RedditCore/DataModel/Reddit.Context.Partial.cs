using System.Data.Entity.Core.EntityClient;
using Ninject;

namespace RedditCore.DataModel
{
    public partial class RedditEntities
    {
        [Inject]
        public RedditEntities(EntityConnection conn)
            : base(conn, false)
        {

        }
    }
}
