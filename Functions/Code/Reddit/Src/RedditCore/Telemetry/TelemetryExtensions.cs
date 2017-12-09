namespace RedditCore.Telemetry
{
    public static class TelemetryExtensions
    {
        /// <summary>
        /// Tracks a dependency and records the time the dependency was in-use.  Timer will stop when the object is disposed.
        /// </summary>
        /// <param name="telemetryClient"></param>
        /// <param name="dependencyName">Basic grouping.  Examples: HTTP remote host name.</param>
        /// <param name="dependencyData">General data for grouping.  Examples: Full HTTP URL, Stored procedure name, etc</param>
        /// <param name="dependencyType">Type of dependency.  Example: HTTP, StoredProcedure, etc</param>
        /// <param name="successDefault">Default is false and user should set to true when operation being timed has suceeded.</param>
        /// <returns></returns>
        public static DependencyTimer StartTrackDependency(
            this ITelemetryClient telemetryClient,
            string dependencyName,
            string dependencyData,
            string dependencyType,
            bool successDefault = false)
        {
            return new DependencyTimer(telemetryClient, dependencyName, dependencyData, dependencyType, successDefault);
        }
    }
}
