using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using System.Collections.Generic;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    public class RetrieveSocialGistApiKey : BaseAction
    {
        private const string SocialGistApiGenerationTemplate = "PowerBI";
        public const string SocialGistApiKeyParameter = "SocialGistApiKey";

        // registration details
        public const string SocialgistRegistrationFirstName = "FirstName";
        public const string SocialgistRegistrationLastName = "LastName";
        public const string SocialgistRegistrationEmailAddress = "EMail";
        public const string SocialgistRegistrationJobTitle = "JobTitle";
        public const string SocialgistRegistrationCompanyName = "CompanyName";
        public const string SocialgistRegistrationDescriptionOfUse = "DescriptionOfUse";
        public const string SocialgistRegistrationAcceptCorrespondenceTerms = "AcceptCorrespondenceTerms";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var errors = new List<string>();
            var firstName = request.DataStore.GetValue(SocialgistRegistrationFirstName);
            if (string.IsNullOrWhiteSpace(firstName))
            {
                errors.Add(FormatRequiredParameterMessage("First Name"));
            }

            var lastName = request.DataStore.GetValue(SocialgistRegistrationLastName);
            if (string.IsNullOrWhiteSpace(lastName))
            {
                errors.Add(FormatRequiredParameterMessage("Last Name"));
            }

            var email = request.DataStore.GetValue(SocialgistRegistrationEmailAddress);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(FormatRequiredParameterMessage("Email Address"));
            }

            var jobTitle = request.DataStore.GetValue(SocialgistRegistrationJobTitle);
            if (string.IsNullOrWhiteSpace(jobTitle))
            {
                errors.Add(FormatRequiredParameterMessage("Job Title"));
            }

            var companyName = request.DataStore.GetValue(SocialgistRegistrationCompanyName);
            if (string.IsNullOrWhiteSpace(companyName))
            {
                errors.Add(FormatRequiredParameterMessage("Company Name"));
            }

            var descriptionOfUse = request.DataStore.GetValue(SocialgistRegistrationDescriptionOfUse);
            if (string.IsNullOrWhiteSpace(descriptionOfUse))
            {
                errors.Add(FormatRequiredParameterMessage("Description of Use"));
            }

            if (!bool.TryParse(request.DataStore.GetValue(SocialgistRegistrationAcceptCorrespondenceTerms), out var acceptCorrespondenceTerms))
            {
                errors.Add(FormatRequiredParameterMessage("Acceptance of Correspondence Terms"));
            }

            if (errors.Count != 0)
            {
                var errorMessage = string.Join("<br/>", errors);
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    errorMessage
                );
            }

            var descriptionPayload = DescriptionFromFields(
                firstName,
                lastName,
                email,
                jobTitle,
                companyName,
                acceptCorrespondenceTerms,
                descriptionOfUse
            );

            try
            {
                var socialGistApiKey = await RetrieveKey(
                    Constants.SocialGistProvisionKeyUrl,
                    Constants.SocialGistProvisionKeyUserName,
                    Constants.SocialGistProvisionKeyPassphrase,
                    descriptionPayload
                );
                if (socialGistApiKey == null)
                {
                    // the only way this can happen is in KeyFromJson - either the json format has changed, or we did not get json back, or something about it confused json.net's parser.
                    var exception =
                        new Exception(
                            "An error occurred contacting Socialgist for your Reddit API key.  The response from Socialgist was unexpected and parsing failed.");
                    return new ActionResponse(
                        ActionStatus.Failure,
                        null,
                        exception,
                        DefaultErrorCodes.DefaultErrorCode,
                        "An error occurred contacting Socialgist for your Reddit API key"
                    );
                }
                // currently returns no value, which we then use and place into the AzureFunction AppSetting step.  Once you populate this with the real value from SocialGist, you shouldn't have to change
                // the init.json
                request.DataStore.AddToDataStore(
                    SocialGistApiKeyParameter,
                    socialGistApiKey,
                    DataStoreType.Private
                );
                return new ActionResponse(ActionStatus.Success);
            }
            catch (Exception e)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    e,
                    DefaultErrorCodes.DefaultErrorCode,
                    "An error occurred contacting Socialgist for your Reddit API key"
                );
            }
        }

        private string FormatRequiredParameterMessage(
            string parameterDescription
        )
        {
            return
                $"The required field {parameterDescription} was not provided in the Socialgist account registration page.";
        }

        public async Task<string> RetrieveKey(
            string socialGistKeyManagerUri,
            string socialGistProvisionKeyUserName,
            string socialGistProvisionKeyPassphrase,
            string descriptionPayload
        )
        {
            var queryParameters = new Dictionary<string, string>
            {
                {"template", SocialGistApiGenerationTemplate},
                {"rt", "json"},
                {"description", descriptionPayload}
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            using (var client = new HttpClient())
            {

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, socialGistKeyManagerUri);
                httpRequestMessage.Headers.Authorization =
                    BasicAuthenticationFromUsernameAndPassphrase(socialGistProvisionKeyUserName,
                        socialGistProvisionKeyPassphrase);

                var postPayload = new FormUrlEncodedContent(queryParameters);
                httpRequestMessage.Content = postPayload;

                var response = await client.SendAsync(httpRequestMessage);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Unable to contact Social Gist key management service.");
                }
                var jsonBody = await response.Content.ReadAsStringAsync();
                return KeyFromJson(jsonBody);
            }
        }

        private AuthenticationHeaderValue BasicAuthenticationFromUsernameAndPassphrase(
            string socialGistProvisionKeyUserName,
            string socialGistProvisionKeyPassphrase
        )
        {
            return new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    Encoding.ASCII.GetBytes($"{socialGistProvisionKeyUserName}:{socialGistProvisionKeyPassphrase}")
                )
            );
        }

        public string KeyFromJson(string jsonBody)
        {
            try
            {
                var obj = JsonUtility.GetJsonObjectFromJsonString(jsonBody);
                var nodes = obj.SelectToken("$.response.Keys.Key[0]");
                return nodes?.Value<string>("Key");
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string DescriptionFromFields(
            string firstName,
            string lastName,
            string email,
            string jobTitle,
            string companyName,
            bool acceptCorrespondenceTerms,
            string descriptionOfUse
        )
        {
            var description = new SocialgistDescription
            {
                Session = RandomGenerator.GetRandomHexadecimal(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Position = jobTitle,
                Company = companyName,
                IsOkayToContact = (acceptCorrespondenceTerms ? "1" : "0"), // a formatter would be better, but time is of the essence TODO: replace
                Use = descriptionOfUse
            };

            return JsonConvert.SerializeObject(description);
        }
    }

    public class SocialgistDescription
    {
        [JsonProperty("session")]
        public string Session { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("use")]
        public string Use { get; set; }

        [JsonProperty("isoktocontact")]
        public string IsOkayToContact { get; set; }
    }

}

