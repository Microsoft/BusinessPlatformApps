using Ninject.Modules;

namespace RedditCore.DocumentFilters
{
    internal class DocumentFiltersModule : NinjectModule
    {
        private readonly bool ingestOnlyUserDefinedEntityDocuments;

        internal DocumentFiltersModule(IConfiguration configuration)
        {
            this.ingestOnlyUserDefinedEntityDocuments = configuration.IngestOnlyDocumentsWithUserDefinedEntities;
        }

        public override void Load()
        {
            if(this.ingestOnlyUserDefinedEntityDocuments)
            {
                Bind<IDocumentFilter>().To<ContainsUserDefinedEntitiesFilter>();
            }
        }
    }
}
