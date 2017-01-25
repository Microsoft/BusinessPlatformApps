using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using Microsoft.Rest;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string code = request.DataStore.GetValue("code");
            string aadTenant = request.DataStore.GetValue("AADTenant");
            string oauthType = (request.DataStore.GetLastValue("oauthType") ?? string.Empty).ToLowerInvariant();
            string api;
            string clientId;
            string tokenUrl;

            switch (oauthType)
            {
                case "powerbi":
                    tokenUrl = string.Format(Constants.AzureTokenUri, aadTenant);
                    clientId = Constants.MicrosoftClientIdPowerBI;
                    api = Constants.PowerBIService;
                    break;
                case "mscrm":
                    api = Constants.AzureManagementCoreApi;
                    clientId = Constants.MsCrmClientId;
                    tokenUrl = string.Format(Constants.AzureTokenUri, aadTenant);
                    break;

                case "keyvault":
                    api = Constants.AzureManagementCoreApi;
                    clientId = Constants.MicrosoftClientIdCrm;
                    tokenUrl = string.Format(Constants.AzureTokenUri, aadTenant);
                    break;
                default:
                    api = Constants.AzureManagementCoreApi;
                    clientId = Constants.MicrosoftClientId;
                    tokenUrl = string.Format(Constants.AzureTokenUri, aadTenant);
                    break;
            }

            JObject primaryResponse = null;
            JObject obj = null;

            using (HttpClient client = new HttpClient())
            {

                var builder = GetTokenUri(code, api, request.Info.WebsiteRootUrl, clientId);
                var content = new StringContent(builder.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                var response = await client.PostAsync(new Uri(tokenUrl), content).Result.Content.ReadAsStringAsync();

                primaryResponse = JsonUtility.GetJsonObjectFromJsonString(response);
                obj = new JObject(new JProperty("AzureToken", primaryResponse));

                if (primaryResponse.SelectToken("error") != null)
                {
                    return new ActionResponse(ActionStatus.Failure, obj, null,
                        DefaultErrorCodes.DefaultLoginFailed,
                        primaryResponse.SelectToken("error_description")?.ToString());
                }
            }


            switch (oauthType)
            {
                case "powerbi":
                    var tenantId = new JwtSecurityToken(primaryResponse["id_token"].ToString())
                                                       .Claims.First(e => e.Type.ToLowerInvariant() == "tid")
                                                       .Value;
                    var directoryName = new JwtSecurityToken(primaryResponse["id_token"].ToString())
                                                       .Claims.First(e => e.Type.ToLowerInvariant() == "unique_name")
                                                       .Value.Split('@').Last();
                    request.DataStore.AddToDataStore("DirectoryName", directoryName);
                    request.DataStore.AddToDataStore("PowerBITenantId", tenantId);
                    break;
                case "keyvault":
                    request.DataStore.AddToDataStore("AzureTokenKV", primaryResponse);
                    break;
                case "mscrm":
                    JObject crmToken = RetrieveCrmToken(primaryResponse["refresh_token"].ToString(), request.Info.WebsiteRootUrl, request.DataStore);
                    request.DataStore.AddToDataStore("MsCrmToken", crmToken);
                    request.DataStore.AddToDataStore("AzureToken", primaryResponse);
                    break;
                default:
                    request.DataStore.AddToDataStore("AzureToken", primaryResponse);
                    break;
            }



            return new ActionResponse(ActionStatus.Success, obj, true);

        }

        private JObject RetrieveCrmToken(string refreshToken, string websiteRootUrl, DataStore dataStore)
        {
            string tokenUrl = string.Format(Constants.AzureTokenUri, dataStore.GetValue("AADTenant"));

            using (HttpClient httpClient = new HttpClient())
            {
                // ms crm token
                string token = GetTokenUri2(refreshToken, Constants.MsCrmResource, websiteRootUrl, Constants.MsCrmClientId);
                StringContent content = new StringContent(token);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                string response = httpClient.PostAsync(new Uri(tokenUrl), content).Result.Content.AsString();

                return JsonUtility.GetJsonObjectFromJsonString(response);
            }
        }

        private static StringBuilder GetTokenUri(string code, string uri, string rootUrl, string clientId)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", clientId},
                {"client_secret", Uri.EscapeDataString(Constants.MicrosoftClientSecret)},
                {"resource", Uri.EscapeDataString(uri)},
                {"redirect_uri", Uri.EscapeDataString(rootUrl + Constants.WebsiteRedirectPath)},
                {"grant_type", "authorization_code"}
            };

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }
            return builder;
        }

        private string GetTokenUri2(string code, string uri, string rootUrl, string clientId)
        {
            return $"refresh_token={code}&" +
                   $"client_id={clientId}&" +
                   $"client_secret={Uri.EscapeDataString(Constants.MicrosoftClientSecret)}&" +
                   $"resource={Uri.EscapeDataString(uri)}&" +
                   $"redirect_uri={Uri.EscapeDataString(rootUrl + Constants.WebsiteRedirectPath)}&" +
                   "grant_type=refresh_token";
        }
    }
}
