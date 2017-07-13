using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.ActivityLogs
{
    [Export(typeof(IAction))]
    public class SetOutputAsPBI : BaseAction
    {
        public async Task<JObject> GetEncryptedPBITokens(ActionRequest request, AzureHttpClient ahc)
        {
            var posturi = "https://ms.portal.azure.com/api/Settings/Select?SettingsPortalInstance=mpac&MigrateUserSettings=true";
            string postbody = "[\"CrossTenant\",\"DashboardInventory\",\"Favorites\",\"FileUpload\",\"General\",\"HubsExtension\",\"Startboard\"]";
            HttpResponseMessage postresponse = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, posturi, postbody);

            string code = request.DataStore.GetValue("codepowerbi");
            //string state = request.DataStore.GetValue("statepowerbi");
            string session_state = request.DataStore.GetValue("sessionstatepowerbi");

            var uri2 = "https://main.streamanalytics.ext.azure.com/api/PowerBI/GetOAuthUrl?portalBaseUri=https%3A%2F%2Fms.portal.azure.com&{}";
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, uri2, "{}");
            string responseString = await response.Content.ReadAsStringAsync();
            string tempstate = responseString.Substring(responseString.IndexOf("state=") + 6);
            string state = tempstate.Substring(0, tempstate.IndexOf("&"));
            string tempclient = responseString.Substring(responseString.IndexOf("client_id=") + 10);
            string clientid = tempclient.Substring(tempclient.IndexOf("&"));
            string token = ahc.Token;
            Dictionary<string, string> headers2 = ahc.Headers;
            headers2.Add("x-ms-client-request-id", clientid);
            AzureHttpClient ahc2 = new AzureHttpClient(token, headers2);

            var uri = $"https://main.streamanalytics.ext.azure.com/api/PowerBI/GetTokens?authUrl=https%3A%2F%2Fms.portal.azure.com%2FTokenAuthorize%3Fcode%3D{code}%26state%3D{state}%26session_state%3D{session_state}";
            HttpResponseMessage response2 = await ahc2.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, uri, "{}");
            string response2String = await response2.Content.ReadAsStringAsync();
            return JsonUtility.GetJObjectFromStringValue(response2String);
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azure_access_token = request.DataStore.GetJson("AzureToken", "access_token");
            var pbi_refresh_token2 = request.DataStore.GetJson("PBIToken", "refresh_token");
            var pbi_refresh_token = "AAEAFH88ZfrkO2qofcWCIte8v7WEY63oAQClihBWkCKWrGtELPkLMqknQBuFORxDUuHwwdCavDmV5QNaAU48BAs8SQeXkLXGCbnzZgaFB1gZMYYJsNAjCmBxzWi/GykL9FXP80IIFgb1FFjypxxMCqrm66qAaJA82GbBK/8q41m1d/gr0i1D4BRN9s9CffYZ1KRa0Si4TMa5j8dLpdNnJmmhs1/mYnj0DvyppLwEHe3ZF2TNhk9kcaQAnRfoVHvDLs9mT7EV9w/Vgh3+XJuD7rVpxyoAN8QHwMd7PRSWRHP9t3EZ0b/lYYo1bfaG/H5cb28sDL9cXux4EH1jAHUkXsNRsCR/WWN4wZAFxwuqWRajPF++3kQnxcdqABCIVPFqCyaPuRVZljketFQiBAAEGE9nyvyI9L08U0zKYG0FVmcdBMkPB73SFypc5vvPoJ5bXbAOUWWiLZs9JwxgTgm9jmotad7/sQ9RuPuIdKfMxO93vXjc98W0OK0RoAiY/9vF4Sq7geqkfB2Da83Kfn+rVjV2eO95RD4/w9VoAiymNY4wiEgQn8EB/GY8Emf5zTdHa2IVSd1Vqn1Y/ds2KZnLxY/2YwxS3vMxFGj/LrbjSThwB5tlRtTIeZ35Kj/7hLucera5wibPUw+EdhLNAr91cwyJ2pBlOe5kS30ysnzJfT6fiTTxGXCo4VdVuldwFVvjLWrq9nDxouPFvzJ4BHYQWnpS981ONkJETFoALl7cUQrX6oC46m7jW2RRMvf6SMxpltF3K35y9mMFE/isqvDBlBRbtd5s0C3XqtZtPjBkmDKdshc2Sa79VR6cVUyW6YvLF++tWMPCfkchN1cUjQod0k1ymkbAWbhvDdzxtM9jE0JWb9g0sTITOMITenSEqApnK9p1d6rt0sV0KYfZJgLOzX+qj4kX0JV5hWGfyfqRnEaYWjvY5W+F9kbsY/Gl2W0TOYsicmcP5Kzjjfzyx1b12QqD3xq2yzTlZh/LIXx5RFCJhh3FMlfc33qDjcEBB0Fe3qcpioivCtjKz06S1w13YTLiGPvVYXw6GofwUTq+V8tTQmZ4RT2fUSp0FOCwTL4VBElIS0Iwkx3cmQNMyFkk3WuT6wDty/a5hRPgQ+pdirJ1B/Fc9cgVRYC5hN08FykyDcbukDtQIRKge4IEZ9CjRpLW3su0PlLwqn/wdratg8Qnpz1zHelrXTGEAnYFZ8iUNENa8zywC+jLG45cjwYtO54soyr1jxK7KjL/q6LrQJcg0ozyiFAcqAj2hjTxd9ajMhB+gQ9Z8Vr+0flJLTvYqaPA3JcFkuc+Rie27t+y1PJSbDQ+V+FqlmFkRjN5mNMm7nAu0djGHSN7OGiWSRA7fbHEu0wE4ib5wcDP0jd8zpWulnu1QK2/nUOjekujCF5qOa/Q+V7wm9nWrEgXj1k2gCGrVZCn4p2B9c5B5/Of0ADFSC3uyaJcbWFxirKYPJa5nLeMjMHNR5dg7QS8jr4MWcRBDEuRQDMmIMCJU0j1ExzOR5aW2eyaLe6f/YF5VrHsTTTfveW6ih7tDNYeprxtPAEzBOCSOpNlpe0ur93GFuDem1m4HIvBWKKozX/bNW0coJ/oADJWjhIBolAOZg8OWAxRMlAprAkaCbl5Zuhfs3wnhUlwnvqZOJ81CXEYRpOnlQA6P8ZOvqkB1Qm/MEJx5zmHWjq0TkavtN461yaWLbYg9GrCstE+yr8B0/2b9oKTYBBYFJxT1Mc0zOYy8ZJOs04ttoW75fUz+jw+/INH";
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var jobName = request.DataStore.GetValue("jobName");
            string uri = $"https://main.streamanalytics.ext.azure.com/api/Outputs/PutOutput?fullResourceId=%2Fsubscriptions%2F{subscription}%2FresourceGroups%2F{resourceGroup}%2Fproviders%2FMicrosoft.StreamAnalytics%2Fstreamingjobs%2F{jobName}&subscriptionId={subscription}&resourceGroupName={resourceGroup}&jobName={jobName}&componentType=&componentName=";
            string uri2 = "https://main.streamanalytics.ext.azure.com/api/Outputs/PutOutput?fullResourceId=%2Fsubscriptions%2F657eb4a4-2e7c-485c-aee6-2816aef905c5%2FresourceGroups%2Fminint-cebcjkutest%2Fproviders%2FMicrosoft.StreamAnalytics%2Fstreamingjobs%2FLancesStreamAnalyticsJob&subscriptionId=657eb4a4-2e7c-485c-aee6-2816aef905c5&resourceGroupName=minint-cebcjkutest&jobName=LancesStreamAnalyticsJob&componentType=&componentName=";
            string input = request.DataStore.GetValue("inputAlias");
            string output = request.DataStore.GetValue("outputAlias");
            var body = $"{{\"properties\":{{\"dataSource\":{{\"outputDocumentDatabaseSource\":{{}},\"outputTopicSource\":{{}},\"outputQueueSource\":{{}},\"outputEventHubSource\":{{}},\"outputSqlDatabaseSource\":{{}},\"outputBlobStorageSource\":{{}},\"outputTableStorageSource\":{{}},\"outputPowerBISource\":{{\"dataSet\":\"testdataset123\",\"table\":\"testtable123\",\"groupId\":\"\",\"groupName\":\"My Workspace\",\"refreshToken\":\"{pbi_refresh_token2}\",\"tokenUserDisplayName\":\"Lance LaMotte\",\"tokenUserPrincipalName\":\"t-lalamo@microsoft.com\"}},\"outputDataLakeSource\":{{}},\"outputIotGatewaySource\":{{}},\"type\":\"PowerBI\"}},\"serialization\":{{}}}},\"createType\":\"None\",\"id\":null,\"location\":\"Australia East\",\"name\":\"SHOWUP2\",\"type\":\"PowerBI\"}}";
            Dictionary<string, string> headers = new Dictionary<string, string>();
            string clientsessionid = "ed03adbd554a47c68d8bf796c5b5be8b";
            headers.Add("x-ms-client-session-id", clientsessionid);
            AzureHttpClient ahc = new AzureHttpClient(azure_access_token, headers);
            JObject encryptedTokens = await GetEncryptedPBITokens(request, ahc);
            HttpResponseMessage response = await ahc.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Put, uri2, body);
            return response.IsSuccessStatusCode ? new ActionResponse(ActionStatus.Success) : new ActionResponse(ActionStatus.Failure);

        }
    }
}