using System.Collections.Generic;
using RedditCore.DataModel;

namespace RedditCore.SocialGist
{
    public interface IUserDefinedEntityFinder
    {
        IEnumerable<UserDefinedEntity> FindAllUserDefinedEntities(IEnumerable<IDocument> documents);
    }
}
