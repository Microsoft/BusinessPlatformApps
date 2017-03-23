using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.CDM
{
    [Export(typeof(IAction))]
    public class DeleteEnvironment : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var environmentIds = request.DataStore.GetAllValues("EnvironmentIds");

            AzureHttpClient client = new AzureHttpClient(azureToken);


            foreach(var environment in environmentIds)
            {
                var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Delete, $"https://management.azure.com/providers/Microsoft.PowerApps/environments/{environment}?api-version=2016-11-01", "{}");
                var responseString = await response.Content.ReadAsStringAsync();
                var responseParsed = JsonUtility.GetJsonObjectFromJsonString(responseString);

               //if(!response.IsSuccessStatusCode)
               // {
               //     return new ActionResponse(ActionStatus.Failure);
               // }
            }
           
           
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
