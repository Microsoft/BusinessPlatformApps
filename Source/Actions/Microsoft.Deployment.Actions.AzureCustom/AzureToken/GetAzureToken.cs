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

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureToken : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string code = request.DataStore.GetValue("code");
            string aadTenant = request.DataStore.GetValue("AADTenant");
            string oauthType = request.DataStore.GetValue("oauthType").ToLowerInvariant();
            string api;
            string clientId;
            string tokenUrl;

            /*
                 var builder = GetTokenUri(refreshToken, Constants.AzureManagementCoreApi, request.Info.WebsiteRootUrl);
            var content = new StringContent(builder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(new Uri(tokenUrl), content).Result.Content.ReadAsStringAsync();

             */

            switch (oauthType)
            {
                case "mscrm":
                    api = Constants.MsCrmResource;
                    clientId = Constants.MsCrmClientId;
                    tokenUrl = Constants.MsCrmToken;
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


            HttpClient client = new HttpClient();

            var builder = GetTokenUri(code, api, request.Info.WebsiteRootUrl, clientId);
            var content = new StringContent(builder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(new Uri(tokenUrl), content).Result.Content.ReadAsStringAsync();

            var primaryResponse = JsonUtility.GetJsonObjectFromJsonString(response);
            var obj = new JObject(new JProperty("AzureToken", primaryResponse));

            if (oauthType.EqualsIgnoreCase("keyvault"))
            {
                builder = GetTokenUri2(primaryResponse["refresh_token"].ToString(), Constants.AzureKeyVaultApi, request.Info.WebsiteRootUrl, clientId);
                content = new StringContent(builder.ToString());
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                response = await client.PostAsync(new Uri(tokenUrl), content).Result.Content.ReadAsStringAsync();

                var secondaryResponse = JsonUtility.GetJsonObjectFromJsonString(response);

                request.DataStore.AddToDataStore("kvToken", secondaryResponse);
            }


            if (primaryResponse.SelectToken("error") != null)
            {
                return new ActionResponse(ActionStatus.Failure, obj, null,
                    DefaultErrorCodes.DefaultLoginFailed,
                    primaryResponse.SelectToken("error_description")?.ToString());
            }

            request.DataStore.AddToDataStore("AzureToken", primaryResponse);

            return new ActionResponse(ActionStatus.Success, obj, true);
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

        private static StringBuilder GetTokenUri2(string code, string uri, string rootUrl, string clientId)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "refresh_token", code},
                {"client_id", clientId},
                {"client_secret", Uri.EscapeDataString(Constants.MicrosoftClientSecret)},
                {"resource", Uri.EscapeDataString(uri)},
                {"redirect_uri", Uri.EscapeDataString(rootUrl + Constants.WebsiteRedirectPath)},
                {"grant_type", "refresh_token"}
            };

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }
            return builder;
        }
    }
}
