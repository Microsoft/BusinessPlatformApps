using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.StreamAnalytics
{
    public class StreamAnalyticsClaims
    {
        [JsonProperty("aud")]
        public string Aud;

        [JsonProperty("iss")]
        public string Iss;

        [JsonProperty("iat")]
        public string Iat;

        [JsonProperty("nbf")]
        public string Nbf;

        [JsonProperty("exp")]
        public string Exp;

        [JsonProperty("ver")]
        public string Ver;

        [JsonProperty("http://schemas.microsoft.com/identity/claims/tenantid")]
        public string TenantId;

        [JsonProperty("http://schemas.microsoft.com/claims/authnmethodsreferences")]
        public string Authnmethodsreferences;

        [JsonProperty("http://schemas.microsoft.com/identity/claims/objectidentifier")]
        public string ObjectIdentifier;

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn")]
        public string Upn;

        [JsonProperty("puid")]
        public string Puid;

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")]
        public string NameIdentifier;

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname")]
        public string Givenname;

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname")]
        public string Surname;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("groups")]
        public string Groups;

        [JsonProperty("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")]
        public string ClaimsName;

        [JsonProperty("appid")]
        public string AppId;

        [JsonProperty("appidacr")]
        public string AppIdAcr;

        [JsonProperty("http://schemas.microsoft.com/identity/claims/scope")]
        public string Scope;

        [JsonProperty("http://schemas.microsoft.com/claims/authnclassreference")]
        public string Authnclassreference;
    }
}
