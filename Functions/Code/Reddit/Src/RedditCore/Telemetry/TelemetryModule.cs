using Ninject.Modules;

namespace RedditCore.Telemetry
{
    internal class TelemetryModule : NinjectModule
    {
        public override void Load()
        {
            // Objects in the default scope (TransientScope) are not disposed when the kernel is disposed
            // Make sure this is in SingletonScope so it is disposed and flushed.
            Bind<ITelemetryClient>().ToProvider<TelemetryClientProvider>().InSingletonScope();
        }
    }
}
