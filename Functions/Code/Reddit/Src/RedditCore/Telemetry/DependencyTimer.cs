using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.ApplicationInsights.DataContracts;

namespace RedditCore.Telemetry
{
    public class DependencyTimer : IDisposable
    {
        private readonly string dependencyName;
        private readonly string commandName;
        private readonly string dependencyType;
        private readonly ITelemetryClient telemetryClient;

        private readonly DateTimeOffset startTime;
        private readonly Stopwatch timer;

        /// <summary>
        /// Creates a new DependencyTimer
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="dependencyName"></param>
        /// <param name="commandName"></param>
        /// <param name="dependencyType"></param>
        /// <param name="successDefault">Default value for IsSuccess.  Default to false and set the property to true after work has succeeded</param>
        public DependencyTimer(ITelemetryClient telemetryClient, string dependencyName, string commandName, string dependencyType, bool successDefault = false)
        {
            this.telemetryClient = telemetryClient;
            this.dependencyName = dependencyName;
            this.commandName = commandName;
            this.dependencyType = dependencyType;

            this.IsSuccess = successDefault;

            this.startTime = DateTimeOffset.Now;
            this.timer = Stopwatch.StartNew();

            this.Properties = new Dictionary<string, string>();
        }

        public string ResultCode { get; set; }

        public bool IsSuccess { get; set; }

        public IDictionary<string, string> Properties { get; private set; }

        public void Dispose()
        {
            var telemetry = new DependencyTelemetry() {
                Name = dependencyName,
                Data = commandName,
                Timestamp = startTime,
                Duration = timer.Elapsed,
                Success = IsSuccess,
                ResultCode = ResultCode,
                Type = dependencyType,
            };

            foreach(var kvp in this.Properties)
            {
                telemetry.Properties.Add(kvp);
            }

            this.telemetryClient.TrackDependency(telemetry);
        }
    }
}
