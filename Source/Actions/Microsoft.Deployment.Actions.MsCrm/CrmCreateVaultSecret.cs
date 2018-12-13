namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Azure.KeyVault.Models;
    using Azure.Management.KeyVault.Models;
    using Azure.Management.Resources;

    using Microsoft.Azure;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Management.KeyVault;
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Enums;
    using Microsoft.Deployment.Common.Helpers;

    using Rest;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmCreateVaultSecret : BaseAction
    {
        string _kvToken;
        string _graphToken;
        string _subscriptionId;
        string _tenantId;

        private const string _crmServicePrincipal = "b861dbcc-a7ef-4219-a005-0e4de4ea7dcf"; // DO NOT CHANGE THIS
        private async Task<string> GetCrmConnectorObjectID(string graphToken, string tenantId)
        {
            Uri servicePointUri = new Uri("https://graph.windows.net");
            Uri serviceRoot = new Uri(servicePointUri, tenantId);
            ActiveDirectoryClient adClient = new ActiveDirectoryClient(serviceRoot, async () => { return graphToken; });

            var princs = await adClient.ServicePrincipals.Where( p => p.AppId.Equals(_crmServicePrincipal)).ExecuteAsync().ConfigureAwait(false);
            var currentPage = princs.CurrentPage;

            if (currentPage.Count > 0)
                return currentPage[0].ObjectId;

            return null;
        }

        private void RetrieveKVToken(string refreshToken, string websiteRootUrl, DataStore dataStore)
        {
            string tokenUrl = string.Format(Common.Constants.AzureTokenUri, dataStore.GetValue("AADTenant"));

            using (HttpClient httpClient = new HttpClient())
            {
                // Key vault token
                string kvTokenUrl = GetTokenUri2(refreshToken, Common.Constants.AzureKeyVaultApi, websiteRootUrl, Common.Constants.MicrosoftClientIdCrm);
                StringContent content = new StringContent(kvTokenUrl);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                string response = httpClient.PostAsync(new Uri(tokenUrl), content).Result.Content.AsString();
                _kvToken = JsonUtility.GetJsonObjectFromJsonString(response)["access_token"].ToString();
            }
        }

        private void RetrieveGraphToken(string refreshToken, string websiteRootUrl, DataStore dataStore)
        {
            string tokenUrl = string.Format(Common.Constants.AzureTokenUri, dataStore.GetValue("AADTenant"));

            using (HttpClient httpClient = new HttpClient())
            {
                // Key vault token
                string graphTokenUrl = GetTokenUri2(refreshToken,"https://graph.windows.net/", websiteRootUrl, Common.Constants.MicrosoftClientIdCrm);
                StringContent content = new StringContent(graphTokenUrl);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                string response = httpClient.PostAsync(new Uri(tokenUrl), content).Result.Content.AsString();
                _graphToken = JsonUtility.GetJsonObjectFromJsonString(response)["access_token"].ToString();
            }
        }

        private static string GetTokenUri2(string code, string uri, string rootUrl, string clientId)
        {
            return $"refresh_token={code}&" +
                   $"client_id={clientId}&" +
                   $"client_secret={Uri.EscapeDataString(Common.Constants.MicrosoftClientSecret)}&" +
                   $"resource={Uri.EscapeDataString(uri)}&" +
                   $"redirect_uri={Uri.EscapeDataString(rootUrl + Common.Constants.WebsiteRedirectPath)}&" +
                   "grant_type=refresh_token";
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureTokenKV", "access_token");
            string refreshToken = request.DataStore.GetJson("AzureTokenKV", "refresh_token");
            string crmToken = request.DataStore.GetJson("MsCrmToken", "access_token");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string vaultName = request.DataStore.GetValue("VaultName") ?? "bpstv-" + RandomGenerator.GetRandomLowerCaseCharacters(12) ;
            string secretName = request.DataStore.GetValue("SecretName") ?? "bpst-mscrm-secret";
            string connectionString = request.DataStore.GetAllValues("SqlConnectionString")[0];
            string organizationId = request.DataStore.GetValue("OrganizationId");

            _subscriptionId = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");

            RetrieveKVToken(refreshToken, request.Info.WebsiteRootUrl, request.DataStore);
            RetrieveGraphToken(refreshToken, request.Info.WebsiteRootUrl, request.DataStore);

            SubscriptionCloudCredentials credentials = new TokenCloudCredentials(_subscriptionId, azureToken);


            // Get user's object ID and tenant ID (in the Azure subscription where the vault is created)
            string oid = null;
            int propCount = 0;
            foreach (var c in new JwtSecurityToken(azureToken).Claims)
            {
                switch (c.Type.ToLowerInvariant())
                {
                    case "oid":
                        oid = c.Value;
                        propCount++;
                        break;
                    case "tid":
                        _tenantId = c.Value;
                        propCount++;
                        break;
                }

                if (propCount >= 2)
                    break;
            }


            // Get user's tenant ID in the CRM implicit subscription
            string crmtenantId = null;
            foreach (var c in new JwtSecurityToken(crmToken).Claims)
            {
                if (c.Type.EqualsIgnoreCase("tid"))
                {
                        crmtenantId = c.Value;
                        break;
                }
            }

            try
            {
                TokenCredentials credentialsKv = new TokenCredentials(azureToken);

                using (KeyVaultManagementClient client = new KeyVaultManagementClient(credentialsKv))
                {
                    client.SubscriptionId = _subscriptionId;

                    // Check if a vault already exists
                    var vaults = client.Vaults.ListByResourceGroup(resourceGroup);

                    foreach (var v in client.Vaults.ListByResourceGroup(resourceGroup))
                    {
                        if (v.Name.EqualsIgnoreCase(vaultName))
                        {
                            client.Vaults.Delete(resourceGroup, vaultName);
                            break;
                        }
                    }


                    // Create the vault
                    string vaultUrl = null;
                    using (ResourceManagementClient resourceClient = new ResourceManagementClient(credentials))
                    {
                        // Set properties
                        VaultProperties p = new VaultProperties()
                        {
                            Sku = new Sku(SkuName.Standard),
                            TenantId = new Guid(_tenantId),
                            AccessPolicies = new List<AccessPolicyEntry>()
                        };

                        // Access policy for the owner
                        AccessPolicyEntry apeOwner = new AccessPolicyEntry();
                        apeOwner.Permissions = new Permissions(new[] { "get", "create", "delete", "list", "update", "import", "backup", "restore" }, new[] { "all" }, new[] { "all" });
                        apeOwner.TenantId = p.TenantId;
                        apeOwner.ObjectId = oid;
                        p.AccessPolicies.Add(apeOwner);

                        // Access policy for the CRM exporter
                        AccessPolicyEntry ape = new AccessPolicyEntry();
                        ape.Permissions = new Permissions(null, new[] { "get" });
                        ape.TenantId = p.TenantId;
                        ape.ObjectId = GetCrmConnectorObjectID(_graphToken, _tenantId).Result;
                        p.AccessPolicies.Add(ape);

                        VaultCreateOrUpdateParameters vaultParams = new VaultCreateOrUpdateParameters()
                        {
                            Location = resourceClient.ResourceGroups.Get(resourceGroup).ResourceGroup.Location,
                            Properties = p
                        };

                        Vault vault = client.Vaults.CreateOrUpdate(resourceGroup, vaultName, vaultParams);
                        vaultUrl    = vault.Properties.VaultUri;
                        Thread.Sleep(15000); // The vault DNS entry isn't there immediatly
                    }

                    // Create the secret
                    KeyVaultClient kvClient = new KeyVaultClient( new TokenCredentials(_kvToken));
                    SecretBundle secret = await kvClient.SetSecretAsync(vaultUrl,
                                                                       secretName,
                                                                       connectionString,
                                                                       new Dictionary<string, string>() { { organizationId, crmtenantId } },
                                                                       null /* Do I need to set a content type? */,
                                                                       new SecretAttributes() { Enabled = true });

                    request.DataStore.AddToDataStore("KeyVault", secret.Id, DataStoreType.Private);
                }
            }
            catch (Exception)
            {
                throw;
            }

            //Log kv name
            request.Logger.LogResource(request.DataStore, vaultName,
                DeployedResourceType.KeyVault, CreatedBy.BPST, DateTime.UtcNow.ToString("o"));

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject(), true);
        }
    }
}