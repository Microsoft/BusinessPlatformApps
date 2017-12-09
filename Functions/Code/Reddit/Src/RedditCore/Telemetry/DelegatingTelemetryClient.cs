using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;

namespace RedditCore.Telemetry
{
    internal class DelegatingTelemetryClient : ITelemetryClient
    {
        private readonly TelemetryClient inner;

        public DelegatingTelemetryClient(TelemetryClient innerDelegate)
        {
            this.inner = innerDelegate;
        }

        public TelemetryContext Context => this.inner.Context;

        public string InstrumentationKey
        {
            get => this.inner.Context.InstrumentationKey;
            set => this.inner.Context.InstrumentationKey = value;
        }

        public bool IsEnabled => !this.inner.IsEnabled();

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null)
        {
            this.inner.TrackEvent(eventName, properties, metrics);
        }

        public void TrackEvent(EventTelemetry telemetry)
        {
            this.inner.TrackEvent(telemetry);
        }

        public void TrackTrace(string message)
        {
            this.inner.TrackTrace(new TraceTelemetry(message));
        }

        public void TrackTrace(string message, SeverityLevel severityLevel)
        {
            this.inner.TrackTrace(new TraceTelemetry(message, severityLevel));
        }

        public void TrackTrace(string message, IDictionary<string, string> properties)
        {
            this.inner.TrackTrace(message, properties);
        }

        public void TrackTrace(string message, SeverityLevel severityLevel, IDictionary<string, string> properties)
        {
            this.inner.TrackTrace(message, severityLevel, properties);
        }

        public void TrackTrace(TraceTelemetry telemetry)
        {
            this.inner.TrackTrace(telemetry);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void TrackMetric(string name, double value, IDictionary<string, string> properties = null)
        {
            this.inner.TrackMetric(name, value, properties);
        }

        public void TrackMetric(MetricTelemetry telemetry)
        {
            this.inner.TrackMetric(telemetry);
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null)
        {
            this.inner.TrackException(exception, properties, metrics);
        }

        public void TrackException(ExceptionTelemetry telemetry)
        {
            this.inner.TrackException(telemetry);
        }

        public void TrackDependency(string dependencyName, string commandName, DateTimeOffset startTime,
            TimeSpan duration, bool success)
        {
            this.inner.TrackDependency(dependencyName, commandName, startTime, duration, success);
        }

        public void TrackDependency(string dependencyTypeName, string target, string dependencyName, string data,
            DateTimeOffset startTime, TimeSpan duration, string resultCode, bool success)
        {
            this.inner.TrackDependency(dependencyTypeName, target, dependencyName, data, startTime, duration,
                resultCode, success);
        }

        public void TrackDependency(DependencyTelemetry telemetry)
        {
            this.inner.TrackDependency(telemetry);
        }

        public void TrackAvailability(string name, DateTimeOffset timeStamp, TimeSpan duration, string runLocation,
            bool success, string message = null, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null)
        {
            this.inner.TrackAvailability(name, timeStamp, duration, runLocation, success, message, properties, metrics);
        }

        public void TrackAvailability(AvailabilityTelemetry telemetry)
        {
            this.inner.TrackAvailability(telemetry);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Track(ITelemetry telemetry)
        {
            this.inner.Track(telemetry);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Initialize(ITelemetry telemetry)
        {
            this.inner.Initialize(telemetry);
        }

        public void TrackPageView(string name)
        {
            this.inner.TrackPageView(name);
        }

        public void TrackPageView(PageViewTelemetry telemetry)
        {
            this.inner.TrackPageView(telemetry);
        }

        public void TrackRequest(string name, DateTimeOffset startTime, TimeSpan duration, string responseCode,
            bool success)
        {
            this.inner.TrackRequest(name, startTime, duration, responseCode, success);
        }

        public void TrackRequest(RequestTelemetry request)
        {
            this.inner.TrackRequest(request);
        }

        public void Flush()
        {
            this.inner.Flush();
        }

        public void Dispose()
        {
            this.inner.Flush();
        }
    }
}
