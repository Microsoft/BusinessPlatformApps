using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.AzureCustom;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class GetAxInstances : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            List<string> axInstances = new List<string>();
            var tenantId = AzureUtility.GetTenantFromToken(request.DataStore.GetJson("AzureToken"));
            string axToken = request.DataStore.GetJson("AxToken", "access_token");

            var jwtToken = new JwtSecurityToken(axToken);
            var userObjectId = jwtToken.Claims.First(e => e.Type == "oid")?.Value;

            if (string.IsNullOrEmpty(userObjectId))
            {
                return new ActionResponse(ActionStatus.Failure, "User Object Id cannot be null. ");
            }

            if (string.IsNullOrEmpty(tenantId))
            {
                return new ActionResponse(ActionStatus.Failure, "Tenant Id cannot be null. ");
            }

            if (string.IsNullOrEmpty(axToken))
            {
                return new ActionResponse(ActionStatus.Failure, "No Dynamics 365 token available.");
            }

            var ctx = new AuthenticationContext(string.Format(Constants.AxLocatorLoginAuthority, tenantId));
            var token = await ctx.AcquireTokenAsync(Constants.AxErpResource, new ClientCredential(Constants.AxLocatorClientId, Constants.AxLocatorSecret));

            HttpResponseMessage response;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.AxLocatorBaseUrl);

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
                client.DefaultRequestHeaders.Add("x-ms-discovery-client-principal-id", userObjectId);
                client.DefaultRequestHeaders.Add("x-ms-discovery-client-tenant-id", tenantId);

                response = client.GetAsync($"/tenantapi/BusinessAppDiscoveryResults(guid'{tenantId}')").Result;
            }

            var content = JsonUtility.GetJsonObjectFromJsonString(await response.Content.ReadAsStringAsync());
            var apps = content["value"]?[0]?["Apps"];

            if (apps != null)
            {
                foreach (var element in apps)
                {
                    axInstances.Add(element["AppOpenUri"].ToString());
                }

                return new ActionResponse(ActionStatus.Success, JsonUtility.Serialize(axInstances));
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}