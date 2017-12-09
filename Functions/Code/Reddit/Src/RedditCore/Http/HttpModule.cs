using Ninject.Modules;

namespace RedditCore.Http
{
    class HttpModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IUrlFinder>().To<UrlFinder>();

            Bind<IHttpClient>().To<HttpClient>();
        }
    }
}
