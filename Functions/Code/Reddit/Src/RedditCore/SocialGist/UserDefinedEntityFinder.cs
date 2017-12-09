using System.Collections.Generic;
using System.Text.RegularExpressions;
using Ninject;
using RedditCore.DataModel;
using RedditCore.DataModel.Repositories;
using RedditCore.Logging;
using static System.String;

namespace RedditCore.SocialGist
{
    internal class UserDefinedEntityFinder : IUserDefinedEntityFinder
    {
        private readonly IRepository<UserDefinedEntityDefinition> definitionRepository;
        private IList<UserDefinedEntityDefinition> entityDefinitions;
        private readonly ILog log;

        [Inject]
        public UserDefinedEntityFinder(IRepository<UserDefinedEntityDefinition> definitionRepository, ILog log)
        {
            this.definitionRepository = definitionRepository;
            this.log = log;
        }

        public IEnumerable<UserDefinedEntity> FindAllUserDefinedEntities(IEnumerable<IDocument> documents)
        {
            log.Verbose("Starting Bring-Your-Own-Entities extraction");

            if (this.entityDefinitions == null)
            {
                log.Verbose("Loading user defined entity definitions");
                // Only retrieve the entity definitions once.
                entityDefinitions = definitionRepository.GetAll();
            }

            var result = new LinkedList<UserDefinedEntity>();

            if (documents != null && entityDefinitions != null)
            {
                // Loop over definitions first to only iterate over the database once and create the regex set once
                foreach (var definition in entityDefinitions)
                {
                    var regex = new Regex(definition.Regex, RegexOptions.IgnoreCase);

                    foreach (var doc in documents)
                    {
                        if (IsNullOrEmpty(doc.Content)) continue;

                        foreach (Match match in regex.Matches(doc.Content))
                        {
                            result.AddLast(new UserDefinedEntity
                            {
                                DocumentId = doc.Id,
                                Entity = definition.EntityValue,
                                EntityType = definition.EntityType,
                                EntityOffset = match.Index,
                                EntityLength = match.Value.Length
                            });
                        }
                    }
                }
            }
            log.Verbose($"BYOEntities finished with {result.Count} items");

            return result;
        }
    }
}