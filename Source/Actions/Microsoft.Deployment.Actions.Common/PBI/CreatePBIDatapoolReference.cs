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
using Microsoft.Deployment.Common;
using Newtonsoft.Json;
using System.Dynamic;

namespace Microsoft.Deployment.Actions.Custom.Ax
{
    [Export(typeof(IAction))]
    public class CreatePBIDatapoolReference : BaseAction
    {
        private const string PBI_CREATE_DATAPOOL = "v1.0/myorg/groups/{0}/dataflows/createReference";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string pbiToken = request.DataStore.GetJson("PBIToken", "access_token").ToString();
            string pbiWorkspaceId = request.DataStore.GetValue("PBIWorkspaceId");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(Constants.PowerBiApiUrl);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pbiToken.ToString());

                dynamic payload = new ExpandoObject();
                payload.name = request.DataStore.GetValue("DatapoolName") + RandomGenerator.GetDateStamp();
                payload.description = request.DataStore.GetValue("DatapoolDescription");
                payload.subscriptionId = request.DataStore.GetValue("KeyVaultSubscriptionId");
                payload.resourceGroupName = request.DataStore.GetValue("KeyVaultResourceGroupName");
                payload.vaultName = request.DataStore.GetValue("KeyVaultName");
                payload.secretPath = request.DataStore.GetValue("KeyVaultSecretPath");

                var response = await client.PostAsync(string.Format(PBI_CREATE_DATAPOOL, pbiWorkspaceId), new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    request.DataStore.AddToDataStore("PBIDatapoolId", JsonUtility.GetJObjectProperty(JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), "id"), DataStoreType.Public);
                    return new ActionResponse(ActionStatus.Success);
                }
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJsonObjectFromJsonString(response.Content.ReadAsStringAsync().Result), null, "DatapoolCreateError", response.StatusCode.ToString());
            }
        }
    }
}