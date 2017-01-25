namespace Microsoft.Deployment.Common.Actions.MsCrm.Model
{
    using Newtonsoft.Json;
    using System.ComponentModel;

#pragma warning disable 0649
    public class MsCrmOrganization
    {
        public string OrganizationName;
        public string OrganizationUrl;
        public string OrganizationId;
        public string ConnectorUrl;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include | DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int ErrorCategory;

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include | DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public int ErrorCode;

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage;

    }
#pragma warning restore 0649
}