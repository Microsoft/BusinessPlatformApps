using Ninject.Modules;
using RedditCore.Logging;

namespace RedditCore.Modules
{
    internal class ConfigurationModule : NinjectModule
    {
        private readonly IConfiguration configuration;
        private readonly IObjectLogger objectLogger;
        private readonly ILog log;

        internal ConfigurationModule(IConfiguration configuration, ILog log, IObjectLogger objectLogger)
        {
            this.configuration = configuration;
            this.log = log;
            this.objectLogger = objectLogger;
        }

        public override void Load()
        {
            Bind<IConfiguration>().ToConstant(configuration);
            Bind<ILog>().ToConstant(log);
            Bind<IObjectLogger>().ToConstant(objectLogger);
        }
    }
}
