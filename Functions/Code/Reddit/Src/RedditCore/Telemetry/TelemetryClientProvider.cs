using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Ninject;
using Ninject.Activation;
using RedditCore.Logging;

namespace RedditCore.Telemetry
{
    internal class TelemetryClientProvider : Provider<ITelemetryClient>
    {
        private readonly IConfiguration configuration;
        private readonly ILog log;

        private const string APPINSIGHTS_KEY = "APPINSIGHTS_INSTRUMENTATIONKEY";

        [Inject]
        public TelemetryClientProvider(IConfiguration configuration, ILog log)
        {
            this.configuration = configuration;
            this.log = log;
        }

        protected override ITelemetryClient CreateInstance(IContext context)
        {
            var key = System.Environment.GetEnvironmentVariable(APPINSIGHTS_KEY);
            TelemetryClient telemetry = null;
            if (key != null)
            {
                log.Verbose($"Using telemetry key {key}");
                TelemetryConfiguration.Active.InstrumentationKey = key;
                telemetry = new TelemetryClient() {InstrumentationKey = key};
            }
            else
            {
                log.Verbose(
                    $"No telemtry key defined.  AppInsights will not be available.  Add a new application configuration with the key {APPINSIGHTS_KEY} and a value of your AppInsights key.");
                telemetry = new TelemetryClient();
            }

            telemetry.Context.Operation.Id = configuration.FunctionInvocationId.ToString();
            telemetry.Context.Operation.Name = configuration.FunctionName;

            return new DelegatingTelemetryClient(telemetry);
        }
    }
}
