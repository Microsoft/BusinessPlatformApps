using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace RedditCore.Telemetry
{
    public interface ITelemetryClient : IDisposable
    {
        TelemetryContext Context { get; }
        string InstrumentationKey { get; set; }
        bool IsEnabled { get; }

        void TrackEvent(string eventName, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null);

        void TrackEvent(EventTelemetry telemetry);
        void TrackTrace(string message);
        void TrackTrace(string message, SeverityLevel severityLevel);
        void TrackTrace(string message, IDictionary<string, string> properties);
        void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties);
        void TrackTrace(TraceTelemetry telemetry);
        void TrackMetric(string name, double value, IDictionary<string, string> properties = null);
        void TrackMetric(MetricTelemetry telemetry);

        void TrackException(Exception exception, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null);

        void TrackException(ExceptionTelemetry telemetry);

        void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime,
            TimeSpan duration, bool success);

        void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success);

        void TrackDependency(DependencyTelemetry telemetry);

        void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation,
            bool success, string message = null, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null);

        void TrackAvailability(AvailabilityTelemetry telemetry);
        void Track(ITelemetry telemetry);
        void Initialize(ITelemetry telemetry);
        void TrackPageView(string name);
        void TrackPageView(PageViewTelemetry telemetry);

        void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode,
            bool success);

        void TrackRequest(RequestTelemetry request);
        void Flush();
    }
}