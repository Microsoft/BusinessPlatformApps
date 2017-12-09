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

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    public class RetrieveSocialGistApiKey : BaseAction
    {
        private const string SocialGistApiGenerationTemplate = "PowerBI";
        public const string SocialGistApiKeyParameter = "SocialGistApiKey";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            try
            {
                var socialGistApiKey = await RetrieveKey(
                    Constants.SocialGistProvisionKeyUrl,
                    Constants.SocialGistProvisionKeyUserName,
                    Constants.SocialGistProvisionKeyPassphrase
                );
                if (socialGistApiKey == null)
                {
                    // the only way this can happen is in KeyFromJson - either the json format has changed, or we did not get json back, or something about it confused json.net's parser.
                    var exception = new Exception("An error occurred contacting SocialGist for your Reddit API key.  The response from SocialGist was unexpected and parsing failed.");
                    return new ActionResponse(
                        ActionStatus.Failure,
                        null,
                        exception,
                        DefaultErrorCodes.DefaultErrorCode,
                        "An error occurred contacting SocialGist for your Reddit API key"
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
                    "An error occurred contacting SocialGist for your Reddit API key"
                );
            }
        }

        public async Task<string> RetrieveKey(
            string socialGistKeyManagerUri,
            string socialGistProvisionKeyUserName,
            string socialGistProvisionKeyPassphrase
        )
        {
            var description = Guid.NewGuid().ToString(); // this should be replaced later with something PII, but only after we have a proper UI and customer agreement to this effect
            var queryParameters = new Dictionary<string, string>
            {
                { "template", SocialGistApiGenerationTemplate },
                { "rt", "json" },
                { "description", description }
            };
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            using (var client = new HttpClient())
            {

                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, socialGistKeyManagerUri);
                httpRequestMessage.Headers.Authorization = BasicAuthenticationFromUsernameAndPassphrase(socialGistProvisionKeyUserName, socialGistProvisionKeyPassphrase);

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
                if (nodes == null)
                {
                    return null;
                }
                return nodes.Value<string>("Key");
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
 
 