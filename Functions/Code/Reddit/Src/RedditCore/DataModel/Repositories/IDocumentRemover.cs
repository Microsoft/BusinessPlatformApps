using System.Collections.Generic;

namespace RedditCore.DataModel.Repositories
{
    internal interface IDocumentRemover
    {
        void RemoveDocuments(IList<IDocument> documents);
    }
}
