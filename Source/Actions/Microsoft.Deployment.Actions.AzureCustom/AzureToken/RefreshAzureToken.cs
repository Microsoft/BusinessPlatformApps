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
using Microsoft.Deployment.Common.Helpers;

using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    [Export(typeof(IActionRequestInterceptor))]
    public class RefreshAzureToken : BaseAction, IActionRequestInterceptor
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string refreshToken = request.DataStore.GetJson("AzureToken")["refresh_token"].ToString();
            string aadTenant = request.DataStore.GetValue("AADTenant");

            string tokenUrl = string.Format(Constants.AzureTokenUri, aadTenant);
            HttpClient client = new HttpClient();

            var builder = GetTokenUri(refreshToken, Constants.AzureManagementCoreApi, request.Info.WebsiteRootUrl);
            var content = new StringContent(builder.ToString());
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            var response = await client.PostAsync(new Uri(tokenUrl), content).Result.Content.ReadAsStringAsync();

            var primaryResponse = JsonUtility.GetJsonObjectFromJsonString(response);
            var obj = new JObject(new JProperty("AzureToken", primaryResponse));

            return new ActionResponse(ActionStatus.Success, obj, true);
        }

        private static StringBuilder GetTokenUri(string refresh_token, string uri, string rootUrl)
        {
            Dictionary<string, string> message = new Dictionary<string, string>
            {
                {"refresh_token", refresh_token},
                {"client_id", Constants.MicrosoftClientId},
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

        public async Task<InterceptorStatus> CanInterceptAsync(IAction actionToExecute, ActionRequest request)
        {
            //TODO - fix to ensure it only works when token has expired
            if (request.DataStore.GetValue("AzureToken") != null && request.DataStore.GetJson("AzureToken")["expires_on"] != null)
            {
               var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureToken")["expires_on"].ToString());
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }
            return InterceptorStatus.Skipped;
        }

        /// <summary>
        /// Update the token (first token found in the data store)
        /// </summary>
        /// <param name="actionToExecute"> the action to execute</param>
        /// <param name="request">the request body</param>
        /// <returns>the response of the intercept</returns>
        public async Task<ActionResponse> InterceptAsync(IAction actionToExecute, ActionRequest request)
        {
            var tokenRefreshResponse = await this.ExecuteActionAsync(request);
            if (tokenRefreshResponse.Status == ActionStatus.Success)
            {
                var datastoreItem = request.DataStore.GetDataStoreItem("AzureToken");
                request.DataStore.UpdateValue(datastoreItem.DataStoreType,datastoreItem.Route, datastoreItem.Key, JObject.FromObject(tokenRefreshResponse.Body)["AzureToken"]);
            }

            return tokenRefreshResponse;
        }

        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(double.Parse(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }
    }
}
