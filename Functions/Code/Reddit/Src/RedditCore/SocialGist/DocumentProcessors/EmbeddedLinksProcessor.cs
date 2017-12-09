using System.Collections.Generic;
using RedditCore.DataModel;
using Ninject;
using RedditCore.DataModel.Repositories;
using RedditCore.Http;
using RedditCore.Logging;

namespace RedditCore.SocialGist
{
    internal class EmbeddedLinksProcessor : IDocumentProcessor
    {
        private readonly IUrlFinder urlFinder;
        private readonly IRepository<EmbeddedUrl> embeddedUrlRepository;
        private readonly ILog log;

        [Inject]
        public EmbeddedLinksProcessor(
            IUrlFinder urlFinder,
            IRepository<EmbeddedUrl> embeddedUrlRepository,
            ILog log)
        {
            this.urlFinder = urlFinder;
            this.embeddedUrlRepository = embeddedUrlRepository;
            this.log = log;
        }

        public void ProcessDocuments(IEnumerable<IDocument> documents)
        {
            var links = new LinkedList<EmbeddedUrl>();

            foreach (var document in documents)
            {
                foreach (var url in this.urlFinder.FindUrls(document.Content))
                {
                    links.AddLast(
                        new EmbeddedUrl()
                        {
                            DocumentId = document.Id,
                            ContentEmbeddedUrl = url.ToString(),
                            ContentEmbeddedUrlDomain = url.Host
                        }
                        );
                }
            }

            if (links.Count != 0)
            {
                log.Info($"Found {links.Count} embedded URLs in the document set");

                // Save all URLs at once for performance reasons
                this.embeddedUrlRepository.Save(links);
            }
            else
            {
                log.Verbose("Found no embedded URLs in the document set");
            }
        }
    }
}
