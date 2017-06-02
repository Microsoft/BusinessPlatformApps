using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Site.Service.Controllers
{

    public class NotifierController : ApiController
    {
        [HttpPost]
        public async Task<HttpResponseMessage> PostDeploymentId()
        {
            try
            {
                var resp = new HttpResponseMessage();
                var body = this.Request.Content.ReadAsStringAsync().Result;

                if (string.IsNullOrEmpty(body))
                {
                    resp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    resp.ReasonPhrase = "Content is null";
                    return resp;
                }

                var content = JsonUtility.GetJsonObjectFromJsonString(body);
                string refreshToken = content["tokens"]?["refresh"].ToString();
                string accessToken = content["tokens"]?["access"].ToString();
                string deploymentId = content["deploymentId"].ToString();

                if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(deploymentId))
                {
                    resp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    resp.ReasonPhrase = "Refresh token, access token or deployment id is null.";
                    return resp;
                }


                string tokenUrl = string.Format(Constants.AzureTokenUri, "common");

                var refreshResponse = await GetToken(refreshToken, tokenUrl, Constants.MicrosoftClientId);
                if (!refreshResponse.IsSuccessStatusCode)
                {
                    resp = new HttpResponseMessage(HttpStatusCode.Forbidden);
                    resp.ReasonPhrase = "Access token could not be refreshed.";
                    return resp;
                }

                string deploymentIdsConnection = Constants.BpstDeploymentIdDatabase;
                var cmd = $"INSERT INTO deploymentids VALUES('{deploymentId}','{DateTime.UtcNow.ToString("o")}')";
                SqlUtility.InvokeSqlCommand(deploymentIdsConnection, cmd, new Dictionary<string, string>());

                resp = new HttpResponseMessage(HttpStatusCode.OK);
                return resp;
            }
            catch
            {
                var resp = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                resp.ReasonPhrase = "An internal error has occurred ";
                return resp;
            }
        }

        private static async Task<HttpResponseMessage> GetToken(string refreshToken, string tokenUrl, string clientId)
        {
            HttpClient client = new HttpClient();

            var builder = GetTokenUri(refreshToken, Constants.AzureManagementCoreApi, clientId);
            var content = new StringContent(builder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(new Uri(tokenUrl), content);
            return response;
        }

        private static StringBuilder GetTokenUri(string refresh_token, string uri, string clientId)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"refresh_token", refresh_token},
                {"client_id", clientId},
                {"client_secret", Uri.EscapeDataString(Constants.MicrosoftClientSecret)},
                {"resource", Uri.EscapeDataString(uri)},
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
