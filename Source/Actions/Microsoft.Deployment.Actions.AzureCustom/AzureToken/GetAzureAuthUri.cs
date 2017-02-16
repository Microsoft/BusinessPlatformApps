using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureToken
{
    [Export(typeof(IAction))]
    public class GetAzureAuthUri : BaseAction
    {
        private void ExtractUserandTenant(string token, out string oId, out string tenantId)
        {
            int propCount = 0;
            oId = tenantId = null;

            foreach (var c in new JwtSecurityToken(token).Claims)
            {
                switch (c.Type.ToLowerInvariant())
                {
                    case "oid":
                        oId = c.Value;
                        propCount++;
                        break;
                    case "tid":
                        tenantId = c.Value;
                        propCount++;
                        break;
                }

                if (propCount >= 2)
                    break;
            }
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var aadTenant = request.DataStore.GetValue("AADTenant");

            string authBase;
            string clientId;
            string resource;

            string oauthType = (request.DataStore.GetLastValue("oauthType") ?? string.Empty).ToLowerInvariant();
            switch (oauthType)
            {
                case "powerbi":
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                    clientId = Constants.MicrosoftClientIdPowerBI;
                    resource = Constants.PowerBIService;
                    break;
                case "mscrm":
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                    clientId = Constants.MsCrmClientId;
                    resource = Constants.AzureManagementApi;
                    break;
                case "keyvault":
                    string azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
                    string subscriptionId = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
                    string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");


                    // Make sure the Key Vault is registered
                    SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscriptionId, azureToken);

                    using (ResourceManagementClient managementClient = new ResourceManagementClient(creds))
                    {
                        ProviderListResult providersResult = managementClient.Providers.List(null);

                        bool kvExists = false;
                        foreach (var p in providersResult.Providers)
                        {
                            if (p.Namespace.EqualsIgnoreCase("Microsoft.KeyVault"))
                            {
                                kvExists = p.RegistrationState.EqualsIgnoreCase("Registered");
                                break;
                            }
                        }

                        AzureOperationResponse operationResponse;
                        if (!kvExists)
                        {
                            operationResponse = managementClient.Providers.Register("Microsoft.KeyVault");
                            if (operationResponse.StatusCode != System.Net.HttpStatusCode.OK || operationResponse.StatusCode != System.Net.HttpStatusCode.Accepted )
                                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "MsCrm_ErrorRegisterKv");

                            Thread.Sleep(10000); // Wait for it to register
                        }
                        
                        string oid ;
                        string tenantID;
                        ExtractUserandTenant(azureToken, out oid, out tenantID);

                        const string vaultApiVersion = "2015-06-01";
                        string tempVaultName = "bpst-" + RandomGenerator.GetRandomLowerCaseCharacters(12);
                        GenericResource genRes = new GenericResource("westus")
                        {
                            Properties = "{\"sku\": { \"family\": \"A\", \"name\": \"Standard\" }, \"tenantId\": \"" + tenantID + "\", \"accessPolicies\": [], \"enabledForDeployment\": true }"
                        };

                        operationResponse = managementClient.Resources.CreateOrUpdate(resourceGroup, new ResourceIdentity(tempVaultName, "Microsoft.KeyVault/vaults", vaultApiVersion), genRes);
                        bool operationSucceeded = (operationResponse.StatusCode == System.Net.HttpStatusCode.OK) || (operationResponse.StatusCode == System.Net.HttpStatusCode.Accepted);
                        Thread.Sleep(15000); // The created vault has an Url. DNS propagation will take a while

                        if (operationSucceeded)
                        {
                            ResourceIdentity resIdent = new ResourceIdentity(tempVaultName, "Microsoft.KeyVault/vaults", vaultApiVersion);
                            operationResponse = managementClient.Resources.Delete(resourceGroup, resIdent);
                            operationSucceeded = (operationResponse.StatusCode == System.Net.HttpStatusCode.OK) || (operationResponse.StatusCode == System.Net.HttpStatusCode.Accepted);
                        }

                        if (!operationSucceeded)
                            return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "MsCrm_ErrorRegisterKv");
                    }

                    clientId = Constants.MicrosoftClientIdCrm;
                    resource = Constants.AzureKeyVaultApi;
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);

                    break;
                default:
                    authBase = string.Format(Constants.AzureAuthUri, aadTenant);
                    clientId = Constants.MicrosoftClientId;
                    resource = Constants.AzureManagementApi;
                    break;
            }

            Dictionary<string, string> message = new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "prompt", "consent" },
                { "response_type", "code" },
                { "redirect_uri", Uri.EscapeDataString(request.Info.WebsiteRootUrl + Constants.WebsiteRedirectPath) },
                { "resource", Uri.EscapeDataString(resource) }
            };

            StringBuilder builder = new StringBuilder();
            builder.Append(authBase);
            foreach (KeyValuePair<string, string> keyValuePair in message)
            {
                builder.Append(keyValuePair.Key + "=" + keyValuePair.Value);
                builder.Append("&");
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(builder.ToString()));
        }
    }
}