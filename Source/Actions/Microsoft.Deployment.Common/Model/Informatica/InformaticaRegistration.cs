using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Informatica
{
    public class InformaticaRegistration
    {
        [JsonProperty("@type")]
        public string Type = "registration";

        [JsonProperty("org")]
        public InformaticaRegistrationOrganization Organization;

        [JsonProperty("registrationCode")]
        public string RegistrationCode = Constants.InformaticaRegistrationCode;

        [JsonProperty("sendEmail")]
        public bool SendEmail = false;

        [JsonProperty("user")]
        public InformaticaRegistrationUser User;

        public InformaticaRegistration(string company, string firstName, string lastName, string password, string email)
        {
            Organization = new InformaticaRegistrationOrganization(company);
            User = new InformaticaRegistrationUser(firstName, lastName, password, email);
        }
    }
}