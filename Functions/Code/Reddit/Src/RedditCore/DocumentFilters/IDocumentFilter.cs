using RedditCore.DataModel;

namespace RedditCore.DocumentFilters
{
    public interface IDocumentFilter
    {
        /// <summary>
        /// Does the actual filter computation.
        /// </summary>
        /// <param name="document"></param>
        /// <returns>True if the document should be filtered.  False otherwise.</returns>
        bool ShouldKeep(IDocument document);
    }
}
