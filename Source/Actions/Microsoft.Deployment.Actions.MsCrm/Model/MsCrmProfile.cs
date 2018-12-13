namespace Microsoft.Deployment.Common.Actions.MsCrm.Model
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

#pragma warning disable 0649
    public class MsCrmRetryPolicy
    {
        public int MaxRetryCount = 5;
        public int IntervalInSeconds = 5;
        public string Backoff = "Simple";
    }

    public class MsCrmEntity
    {
        public string Type;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Settings;
        public MsCrmStatus Status;

        // This is used by retrieve entities
        public string LogicalName;
        public string DisplayName;

        public MsCrmEntity()
        {
            Settings = new Dictionary<string, object>();
        }
    }

    public class MsCrmStatus
    {
        public string Type;
        public string ProfileId;

        public int TotalNotifications;
        public int SuccessNotifications;
        public int FailureNotifications;

        [JsonProperty(ItemConverterType = typeof (IsoDateTimeConverter))]
        public DateTime LastExportDate;

        public string LastExportStatus;
        public string EntityMetadataState;
        public string InitialSyncState;
    }

    public class MsCrmProfile
    {
        public string Id;
        public string Version;
        public string State;
        public string OrganizationUrl;
        public string Name;
        public string OrganizationId;
        
        public MsCrmEntity[] Entities;
        public string DestinationType = "sqlazure";
        public string DestinationKeyVaultUri;
        public string DestinationPrefix = string.Empty;
        public MsCrmRetryPolicy RetryPolicy = new MsCrmRetryPolicy();
        public bool WriteDeleteLog = true;
        public string DestinationSchemaName = string.Empty;
    }
#pragma warning restore 0649
}