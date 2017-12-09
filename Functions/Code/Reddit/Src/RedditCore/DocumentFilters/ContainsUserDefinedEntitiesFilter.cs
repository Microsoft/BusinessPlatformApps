using System.Linq;
using RedditCore.DataModel;
using RedditCore.SocialGist;

namespace RedditCore.DocumentFilters
{
    internal class ContainsUserDefinedEntitiesFilter : IDocumentFilter
    {
        private readonly IUserDefinedEntityFinder userDefinedEntityFinder;

        public ContainsUserDefinedEntitiesFilter( IUserDefinedEntityFinder userDefinedEntityFinder)
        {
            this.userDefinedEntityFinder = userDefinedEntityFinder;
        }

        public bool ShouldKeep(IDocument document)
        {
            // If the document is an invalid comment then we want to keep it around.  This allows us to delete invalid comments from the database later.
            return (document is InvalidComment) || userDefinedEntityFinder.FindAllUserDefinedEntities(new[] { document }).Any();
        }
    }
}
