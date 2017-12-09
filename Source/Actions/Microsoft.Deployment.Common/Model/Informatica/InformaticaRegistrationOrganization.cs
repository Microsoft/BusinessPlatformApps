using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Informatica
{
    public class InformaticaRegistrationOrganization
    {
        [JsonProperty("@type")]
        public string Type = "org";

        [JsonProperty("address1")]
        public string address1 = "power bi unspecified";

        [JsonProperty("city")]
        public string City = "Winthrop";

        [JsonProperty("country")]
        public string Country = "US";

        [JsonProperty("employees")]
        public string Employees = "0_10";

        [JsonProperty("name")]
        public string Name = "";

        [JsonProperty("state")]
        public string State = "WA";

        [JsonProperty("zipcode")]
        public string Zipcode = "98862";

        public InformaticaRegistrationOrganization(string company)
        {
            Name = company;
        }
    }
}