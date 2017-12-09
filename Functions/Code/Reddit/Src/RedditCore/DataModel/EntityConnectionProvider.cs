using System.Data.Entity.Core.EntityClient;
using Ninject;
using Ninject.Activation;

namespace RedditCore.DataModel
{
    internal class EntityConnectionProvider : Provider<EntityConnection>
    {
        private readonly IConfiguration configuration;

        [Inject]
        public EntityConnectionProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        protected override EntityConnection CreateInstance(IContext context)
        {
            var connectionBuilder = new EntityConnectionStringBuilder();
            connectionBuilder.Provider = "System.Data.SqlClient";
            connectionBuilder.ProviderConnectionString = configuration.DbConnectionString;
            connectionBuilder.Metadata = "res://*/DataModel.Reddit.csdl|res://*/DataModel.Reddit.ssdl|res://*/DataModel.Reddit.msl";

            return new EntityConnection(connectionBuilder.ToString());
        }
    }
}
