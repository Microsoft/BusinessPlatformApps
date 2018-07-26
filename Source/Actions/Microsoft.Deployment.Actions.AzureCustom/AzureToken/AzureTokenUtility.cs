using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Hyak.Common.Internals;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    public class AzureTokenUtility
    {
        public static JObject GetTokenForResourceFromExistingToken(string oauthType, string redirect, JToken tokenWithRefresh, string resource)
        {
            JObject tokenObj;
            using (HttpClient httpClient = new HttpClient())
            {
                string tenantId = AzureUtility.GetTenantFromToken(tokenWithRefresh);
                string refreshToken = AzureUtility.GetRefreshToken(tokenWithRefresh);
                string tokenUrl = string.Format(Constants.AzureTokenUri, tenantId);

                var tokenMeta = GetMetaFromOAuthType(oauthType);
                string token = AzureTokenUtility.GetTokenBodyFromRefreshToken(refreshToken, resource, redirect, tokenMeta.ClientId);
                StringContent content = new StringContent(token);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                string response2 = httpClient.PostAsync(new Uri(tokenUrl), content).Result.Content.AsString();
                tokenObj = JsonUtility.GetJsonObjectFromJsonString(response2);
            }

            return tokenObj;
        }

        public static JObject GetTokenForResourceFromCode(string resource, string client, string tenantId, string redirect, string code)
        {
            var meta = new AzureTokenRequestMeta(resource, client);
            return GetTokenForResourceFromCode(meta, tenantId, redirect, code);
        }

        public static JObject GetTokenForResourceFromCode(string oauthType, string tenantId, string redirect, string code)
        {
            var meta = AzureTokenUtility.GetMetaFromOAuthType(oauthType);
            return GetTokenForResourceFromCode(meta, tenantId, redirect, code);
        }

        public static JObject GetTokenForResourceFromCode(AzureTokenRequestMeta meta, string tenantId, string redirect, string code)
        {
            JObject tokenObj;
            using (HttpClient httpClient = new HttpClient())
            {
                string tokenUrl = string.Format(Constants.AzureTokenUri, tenantId);
                string token = AzureTokenUtility.GetTokenBodyFromCode(code, meta.Resource, redirect, meta.ClientId);
                StringContent content = new StringContent(token);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                string response2 = httpClient.PostAsync(new Uri(tokenUrl), content).Result.Content.AsString();
                tokenObj = JsonUtility.GetJsonObjectFromJsonString(response2);
            }

            return tokenObj;
        }

        public static string GetTokenBodyFromCode(string code, string resource, string rootUrl, string clientId)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"code", code},
                {"client_id", clientId},
                {"client_secret", Uri.EscapeDataString(Constants.MicrosoftClientSecret)},
                {"resource", Uri.EscapeDataString(resource)},
                {"redirect_uri", Uri.EscapeDataString(rootUrl + Constants.WebsiteRedirectPath)},
                {"grant_type", "authorization_code"}
            };

            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }
            return builder.ToString();
        }

        public static string GetTokenBodyFromRefreshToken(string refreshToken, string resource, string rootUrl, string clientId)
        {
            return $"refresh_token={refreshToken}&" +
                   $"client_id={clientId}&" +
                   $"resource={Uri.EscapeDataString(resource)}&" +
                   $"redirect_uri={Uri.EscapeDataString(rootUrl + Constants.WebsiteRedirectPath)}&" +
                   "grant_type=refresh_token";
        }

        public static AzureTokenRequestMeta GetMetaFromOAuthType(string oauthType)
        {
            string resource = Constants.AzureManagementCoreApi;
            string clientId = Constants.MicrosoftClientId;

            switch (oauthType)
            {
                case "powerbi":
                    resource = Constants.PowerBIService;
                    clientId = Constants.MicrosoftClientIdPowerBI;
                    break;
                case "mscrm":
                    resource = Constants.MsCrmResource;
                    clientId = Constants.MsCrmClientId;
                    break;
                case "keyvault":
                    resource = Constants.AzureManagementCoreApi;
                    clientId = Constants.MicrosoftClientIdCrm;
                    break;
                case "axerp":
                    resource = Constants.AxErpResource;
                    clientId = Constants.AxClientId;
                    break;
                case "as":
                    resource = Constants.AzureManagementCoreApi;
                    clientId = Constants.ASClientId;
                    break;
                case "o365":
                    resource = Constants.AzureManagementCoreApi;
                    clientId = Constants.Office365ClientId;
                    break;
            }

            return new AzureTokenRequestMeta(resource, clientId);
        }

        public static string GetAzureAuthUri(string oauthType, string redirect, string authBase, string state = "")
        {
            var meta = GetMetaFromOAuthType(oauthType);

            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "client_id", meta.ClientId },
                { "prompt", "consent" },
                { "response_type", "code" },
                { "redirect_uri", Uri.EscapeDataString(redirect) },
                { "resource", Uri.EscapeDataString(meta.Resource) },
                { "state", Uri.EscapeDataString(state) }
            };

            StringBuilder builder = new StringBuilder();
            builder.Append(authBase);
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }

            return builder.ToString();
        }

        public static string GetAuthUriForServicePrincipal(string clientId, string authBase, string redirect)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "prompt", "consent" },
                { "response_type", "code" },
                { "redirect_uri", Uri.EscapeDataString(redirect) },
                { "resource", Uri.EscapeDataString("https://manage.office.com") }
            };

            StringBuilder builder = new StringBuilder();
            builder.Append(authBase);
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }

            return builder.ToString();
        }
    }
}