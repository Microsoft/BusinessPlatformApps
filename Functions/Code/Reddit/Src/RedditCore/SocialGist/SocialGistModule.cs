using System;
using Ninject;
using Ninject.Modules;
using RedditCore.DataModel;
using RedditCore.SocialGist.Model;

namespace RedditCore.SocialGist
{
    internal class SocialGistModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IIds>().To<Ids>();

            Bind<ISocialGist>().To<SocialGist>();

            Bind<IThreadProcessor>().To<ThreadProcessor>();

            Bind<IUserDefinedEntityFinder>().To<UserDefinedEntityFinder>();

            Bind<Func<ResponsePost, Post>>().ToMethod(ctx => ctx.Kernel.Get<ThreadResponsePostTransformer>().Transform);
            Bind<Func<ResponseComment, Comment>>()
                .ToMethod(ctx => ctx.Kernel.Get<ThreadResponseCommentTransformer>().Transform);
        }
    }
}
