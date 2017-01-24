namespace Microsoft.Deployment.Common.Actions.MsCrm.Model
{
    public class MsCrmEndpoints
    {
        public const string ENDPOINT = "https://discovery.crmreplication.azure.net";
        public const string URL_ORGANIZATIONS = "/crm/exporter/metadata/organizations";
        public const string URL_ORGANIZATION_METADATA = "/crm/exporter/metadata/discover";
        public const string URL_ORGANIZATION_CONNECTOR = "/crm/exporter/metadata/connector";
        public const string URL_LOGIN = "/crm/exporter/aad/challenge";
        public const string URL_PROFILES = "/crm/exporter/profiles";
        public const string URL_PROFILES_ACTIVATE = "/crm/exporter/profiles/{0}/activate";
        public const string URL_PROFILES_VALIDATE = "/crm/exporter/profiles/validate";
        public const string URL_ENTITIES = "/crm/exporter/metadata/entities";
    }

}
