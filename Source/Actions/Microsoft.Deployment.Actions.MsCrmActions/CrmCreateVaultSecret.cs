namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Azure.KeyVault.Models;
    using Azure.Management.KeyVault.Models;
    using Azure.Management.Resources;
    using Microsoft.Azure;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Azure.Management.KeyVault;
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Rest;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
    
    [Export(typeof(IAction))]
    public class CrmCreateVaultSecret : BaseAction
    {
        private readonly Guid _crmServicePrincipal = new Guid("b861dbcc-a7ef-4219-a005-0e4de4ea7dcf"); // DO NOT CHANGE THIS
        private string token = null;

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            string kvToken = request.DataStore.GetJson("kvToken")["access_token"].ToString();


            string oid = null;
            string tenantId = null;
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
                        tenantId = c.Value;
                        propCount++;
                        break;
                }

                if (propCount >= 2)
                    break;
            }


            string subscriptionID = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string vaultName = request.DataStore.GetValue("VaultName") ?? "bpst-mscrm-vault";
            string secretName = request.DataStore.GetValue("SecretName") ?? "bpst-mscrm-secret";
            string connectionString = request.DataStore.GetAllValues("SqlConnectionString")[0];
            string organizationId = request.DataStore.GetValue("OrganizationId");

            string secretId = string.Empty;

            try
            {
                SubscriptionCloudCredentials credentials = new TokenCloudCredentials(subscriptionID, azureToken);
                TokenCredentials credentialsKv = new TokenCredentials(azureToken);

                using (KeyVaultManagementClient client = new KeyVaultManagementClient(credentialsKv))
                {
                    client.SubscriptionId = subscriptionID;

                    // Check if a vault already exists
                    Vault vault = null;
                    var vaults = client.Vaults.ListByResourceGroup(resourceGroup);

                    foreach (var v in client.Vaults.ListByResourceGroup(resourceGroup))
                    {
                        if (v.Name.EqualsIgnoreCase(vaultName))
                        {
                            vault = v;
                            break;
                        }
                    }

                    VaultCreateOrUpdateParameters vaultParams = null;
                    // Create the vault
                    if (vault == null)
                    {
                        using (ResourceManagementClient resourceClient = new ResourceManagementClient(credentials))
                        {
                            // Set properties
                            VaultProperties p = new VaultProperties()
                            {
                                Sku = new Sku(SkuName.Standard),
                                TenantId = new Guid(tenantId),
                                AccessPolicies = new List<AccessPolicyEntry>()
                            };

                            // Access policy for the owner
                            AccessPolicyEntry apeOwner = new AccessPolicyEntry();
                            apeOwner.Permissions = new Permissions(new[] { "get", "create", "delete", "list", "update", "import", "backup", "restore" }, new[] { "all" }, new[] { "all" });
                            apeOwner.TenantId = new Guid(tenantId);
                            apeOwner.ObjectId = new Guid(oid);
                            p.AccessPolicies.Add(apeOwner);

                            vaultParams = new VaultCreateOrUpdateParameters()
                            {
                                Location = resourceClient.ResourceGroups.Get(resourceGroup).ResourceGroup.Location,
                                Properties = p
                            };

                            vault = client.Vaults.CreateOrUpdate(resourceGroup, vaultName, vaultParams);
                        }
                    }


                    // Access policy for the CRM exporter
                    AccessPolicyEntry ape = new AccessPolicyEntry();
                    ape.Permissions = new Permissions(null, new[] { "get" });
                    ape.TenantId = new Guid(tenantId);
                    ape.ApplicationId = _crmServicePrincipal;
                    ape.ObjectId = new Guid("a1685f9d-abab-4c93-957c-32ffd34cba2b"); // CRM object id
                    vault.Properties.AccessPolicies.Add(ape);

                    // Update permissions
                    vaultParams = new VaultCreateOrUpdateParameters(vault.Location, vault.Properties);
                    vault = client.Vaults.CreateOrUpdate(resourceGroup, vaultName, vaultParams);

                    // Create the secret
                    KeyVaultClient kvClient = new KeyVaultClient( new TokenCredentials(kvToken));
                    SecretBundle secret = await kvClient.SetSecretAsync(vault.Properties.VaultUri, secretName, connectionString, new Dictionary<string, string>() { { organizationId, tenantId } },
                                                 null, new SecretAttributes() { Enabled = true });

                    request.DataStore.AddToDataStore("KeyVault", secret.Id, DataStoreType.Private);
                    secretId = secret.Id;
                }
            }
            catch (Exception)
            {
                throw;
            }

            return new ActionResponse(ActionStatus.Success, secretId, true);
        }
    }
}