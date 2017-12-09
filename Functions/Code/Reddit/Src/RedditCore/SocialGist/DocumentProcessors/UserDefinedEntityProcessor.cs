using System.Collections.Generic;
using RedditCore.DataModel;
using Ninject;
using RedditCore.DataModel.Repositories;

namespace RedditCore.SocialGist
{
    internal class UserDefinedEntityProcessor : IDocumentProcessor
    {
        private readonly IUserDefinedEntityFinder entityFinder;
        private readonly IRepository<UserDefinedEntity> userDefinedEntityRepository;

        [Inject]
        public UserDefinedEntityProcessor(
            IUserDefinedEntityFinder entityFinder,
            IRepository<UserDefinedEntity> userDefinedEntityRepository)
        {
            this.entityFinder = entityFinder;
            this.userDefinedEntityRepository = userDefinedEntityRepository;
        }

        public void ProcessDocuments(IEnumerable<IDocument> documents)
        {
            var entities = this.entityFinder.FindAllUserDefinedEntities(documents);
            this.userDefinedEntityRepository.Save(entities);
        }
    }
}
