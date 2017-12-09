using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Model.Informatica
{
    public class InformaticaRegistrationUser
    {
        [JsonProperty("@type")]
        public string Type = "user";

        [JsonProperty("firstName")]
        public string FirstName = "";

        [JsonProperty("lastName")]
        public string LastName = "";

        [JsonProperty("password")]
        public string Password = "";

        [JsonProperty("name")]
        public string Name = "";

        public InformaticaRegistrationUser(string firstName, string lastName, string password, string name)
        {
            FirstName = firstName;
            LastName = lastName;
            Password = password;
            Name = name;
        }
    }
}