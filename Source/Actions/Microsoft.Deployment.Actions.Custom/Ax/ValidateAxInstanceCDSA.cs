using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Deployment.Common.Helpers;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class ValidateAxInstanceCDSA : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var axToken = request.DataStore.GetJson("AxToken", "access_token").ToString();
            string instance = request.DataStore.GetValue("AxInstanceName");

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(instance);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", axToken.ToString());

                var response = await client.GetAsync($"data/CDSAIntegrationEntities");

                if (!response.IsSuccessStatusCode && response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), null, "AxWrongPlatform", response.Content.ReadAsStringAsync().Result);
                }

                populateCDSAConfiguration(request, response.Content.ReadAsStringAsync().Result);
            }

            return new ActionResponse(ActionStatus.Success);
        }

        private void populateCDSAConfiguration(ActionRequest request, string response)
        {
            if (JsonUtility.GetJObjectFromJsonString(response).SelectToken("value") != null && JsonUtility.GetJObjectFromJsonString(response).SelectToken("value").Count() > 0)
            {
                var cdsaConfiguration = JsonUtility.Deserialize<Dictionary<string, string>>(JsonUtility.GetJObjectFromJsonString(response).SelectToken("value")[0]);
                string storageAccountName, storageAccountKey, azureSubscriptionId, azureResourceGroupName, keyVaultName, keyVaultSecretName;

                storageAccountName = storageAccountKey = azureSubscriptionId = azureResourceGroupName = keyVaultName = keyVaultSecretName = null;

                cdsaConfiguration.TryGetValue("StorageAccountName", out storageAccountName);
                cdsaConfiguration.TryGetValue("StorageAccountKey", out storageAccountKey);
                cdsaConfiguration.TryGetValue("AzureSubscriptionId", out azureSubscriptionId);
                cdsaConfiguration.TryGetValue("AzureResourceGroupName", out azureResourceGroupName);
                cdsaConfiguration.TryGetValue("KeyVaultName", out keyVaultName);
                cdsaConfiguration.TryGetValue("KeyVaultSecretName", out keyVaultSecretName);

                request.DataStore.AddToDataStore("StorageAccountName", storageAccountName, DataStoreType.Private);
                request.DataStore.AddToDataStore("StorageAccountKey", storageAccountKey, DataStoreType.Private);
                request.DataStore.AddToDataStore("KeyVaultSubscriptionId", azureSubscriptionId, DataStoreType.Private);
                request.DataStore.AddToDataStore("KeyVaultResourceGroupName", azureResourceGroupName, DataStoreType.Private);
                request.DataStore.AddToDataStore("KeyVaultName", keyVaultName, DataStoreType.Private);
                request.DataStore.AddToDataStore("KeyVaultSecretPath", keyVaultSecretName, DataStoreType.Private);
            }
        }
    }
}