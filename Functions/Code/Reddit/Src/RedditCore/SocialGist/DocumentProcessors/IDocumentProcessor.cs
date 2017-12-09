using RedditCore.DataModel;
using System.Collections.Generic;

namespace RedditCore.SocialGist
{
    public interface IDocumentProcessor
    {
        void ProcessDocuments(IEnumerable<IDocument> documents);
    }
}
