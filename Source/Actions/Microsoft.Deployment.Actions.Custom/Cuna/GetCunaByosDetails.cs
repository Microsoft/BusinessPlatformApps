using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.Cuna
{
    [Export(typeof(IAction))]
    public class GetCunaByosDetails : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var userId = request.DataStore.GetValue("userId");
            var contractId = request.DataStore.GetValue("contractId");
            var cunaApiAccessToken = request.DataStore.GetValue("CunaApiAccessToken");
            var pbiToken = request.DataStore.GetJson("PBIToken", "access_token").ToString();
            JwtSecurityToken jwtToken = null;

            try
            {
                jwtToken = new JwtSecurityToken(pbiToken);
            }
            catch(Exception ex)
            {
                return new ActionResponse(ActionStatus.Failure, null, null, "CunaInvalidRequest", ex.Message);
            }

            var customerUpn = jwtToken.Claims.First(c => c.Type == "upn")?.Value;
            var tenantId = jwtToken.Claims.First(c => c.Type == "tid")?.Value;

            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(contractId) || string.IsNullOrWhiteSpace(cunaApiAccessToken) || string.IsNullOrWhiteSpace(customerUpn) || string.IsNullOrWhiteSpace(tenantId))
            {
                return new ActionResponse(ActionStatus.Failure, null, null, "CunaInvalidRequest");
            }

            try
            {
                var response = await GetByosDetails(Constants.CunaApiUrl, userId, contractId, customerUpn, tenantId, cunaApiAccessToken);

                if (response?["Status"] != null)
                {
                    request.DataStore.AddToDataStore("CunaStatus", response["Status"]?.ToString(), DataStoreType.Private);

                    if (response["Status"].ToString().Equals("success", StringComparison.InvariantCultureIgnoreCase) && response["Details"] != null)
                    {
                        request.DataStore.AddToDataStore("DatapoolName", response["Details"]["Name"]?.ToString(), DataStoreType.Private);
                        request.DataStore.AddToDataStore("DatapoolDescription", response["Details"]["Description"]?.ToString(), DataStoreType.Private);
                        request.DataStore.AddToDataStore("KeyVaultSubscriptionId", response["Details"]["SubscriptionID"]?.ToString(), DataStoreType.Private);
                        request.DataStore.AddToDataStore("KeyVaultResourceGroupName", response["Details"]["ResourceGroupName"]?.ToString(), DataStoreType.Private);
                        request.DataStore.AddToDataStore("KeyVaultName", response["Details"]["VaultName"]?.ToString(), DataStoreType.Private);
                        request.DataStore.AddToDataStore("KeyVaultSecretPath", response["Details"]["SecretPath"]?.ToString(), DataStoreType.Private);
                        return new ActionResponse(ActionStatus.Success, response.ToString(), true);
                    }
                    else
                    {
                        return new ActionResponse(ActionStatus.Failure, response, null, "CunaApiCallFailure", response["message"]?.ToString());
                    }
                }
                return new ActionResponse(ActionStatus.Failure, null, null, "CunaApiNoResponse");
            }
            catch(Exception ex)
            {
                return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("CunaApiResponseProcessingFailed", ex.Message));
            }
        }

        public async Task<JObject> GetByosDetails(
            string cunaApiUrl,
            string userId,
            string contractId,
            string customerUpn,
            string tenantId,
            string authToken
        )
        {
            var postData = new Dictionary<string, string>
            {
                {"userId", userId},
                {"contractId", contractId},
                {"customerUpn", customerUpn},
                {"customerTenant", tenantId }
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(cunaApiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Unable to contact Cuna API service.");
                }
                var jsonBody = await response.Content.ReadAsStringAsync();
                return JsonUtility.GetJsonObjectFromJsonString(jsonBody);
            }
        }
    }
}
