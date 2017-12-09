using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    [Export(typeof(IActionRequestInterceptor))]
    public class RefreshAzureToken : BaseAction, IActionRequestInterceptor
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            // Handle Azure token slightly diffrent - depends on client id
            if (request.DataStore.GetValue("MsCrmToken") == null && request.DataStore.GetValue("AzureToken") != null && request.DataStore.GetJson("AzureToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("AzureToken");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("", request.Info.WebsiteRootUrl, dataStoreItem.Value, meta.Resource);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            // Handle Azure token slightly diffrent - depends on client id - use mscrm client id to refresh the token
            // Checks for both tokens in the CRMSalesManagement case
            if (request.DataStore.GetValue("MsCrmToken") != null && request.DataStore.GetValue("AzureToken") != null && request.DataStore.GetJson("AzureToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("AzureToken");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("mscrm");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("mscrm", request.Info.WebsiteRootUrl, dataStoreItem.Value, Constants.AzureManagementCoreApi);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            if (request.DataStore.GetValue("AzureTokenKV") != null && request.DataStore.GetJson("AzureTokenKV", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureTokenKV", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("AzureTokenKV");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("keyvault");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("keyvault", request.Info.WebsiteRootUrl, dataStoreItem.Value, meta.Resource);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            // Checks for crmtoken expiry in CrmSalesManagement
            if (request.DataStore.GetValue("MsCrmToken") != null && request.DataStore.GetJson("MsCrmToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("MsCrmToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("MsCrmToken");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("mscrm");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("mscrm", request.Info.WebsiteRootUrl, dataStoreItem.Value, meta.Resource);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            if (request.DataStore.GetValue("AzureTokenAS") != null && request.DataStore.GetJson("AzureTokenAS", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureTokenAS", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("AzureTokenAS");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("as");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("as", request.Info.WebsiteRootUrl, dataStoreItem.Value, meta.Resource);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            if (request.DataStore.GetValue("AxToken") != null && request.DataStore.GetJson("AxToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AxToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    var dataStoreItem = request.DataStore.GetDataStoreItem("AxToken");
                    var meta = AzureTokenUtility.GetMetaFromOAuthType("axerp");
                    var newToken = AzureTokenUtility.GetTokenForResourceFromExistingToken("axerp", request.Info.WebsiteRootUrl, dataStoreItem.Value, meta.Resource);
                    UpdateToken(dataStoreItem.Value, newToken);
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }


        public async Task<InterceptorStatus> CanInterceptAsync(IAction actionToExecute, ActionRequest request)
        {
            if (request.DataStore.GetValue("AzureToken") != null && request.DataStore.GetJson("AzureToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }

            if (request.DataStore.GetValue("AzureTokenKV") != null && request.DataStore.GetJson("AzureTokenKV", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureTokenKV", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }

            if (request.DataStore.GetValue("MsCrmToken") != null && request.DataStore.GetJson("MsCrmToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("MsCrmToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }

            if (request.DataStore.GetValue("AzureTokenAS") != null && request.DataStore.GetJson("AzureTokenAS", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AzureTokenAS", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }

            if (request.DataStore.GetValue("AxToken") != null && request.DataStore.GetJson("AxToken", "expires_on") != null)
            {
                var expiryDateTime = UnixTimeStampToDateTime(request.DataStore.GetJson("AxToken", "expires_on"));
                if ((expiryDateTime - DateTime.Now).TotalMinutes < 5)
                {
                    return InterceptorStatus.Intercept;
                }
            }

            return InterceptorStatus.Skipped;
        }

        public async Task<ActionResponse> InterceptAsync(IAction actionToExecute, ActionRequest request)
        {
            var tokenRefreshResponse = await this.ExecuteActionAsync(request);
            return tokenRefreshResponse;
        }

        public static DateTime UnixTimeStampToDateTime(string unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(double.Parse(unixTimeStamp)).ToLocalTime();
            return dtDateTime;
        }

        public static void UpdateToken(JToken originalToken, JObject refreshToken)
        {
            foreach(var token in refreshToken)
            {
                originalToken[token.Key] = token.Value;
            }
        }
    }
}