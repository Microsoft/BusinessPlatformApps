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
using Newtonsoft.Json;

namespace Microsoft.Deployment.Actions.AzureCustom.CDM
{
    [Export(typeof(IAction))]
    public class GetAllEnvironments : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");

            AzureHttpClient client = new AzureHttpClient(azureToken);

            var responseWithNames = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, $"https://management.azure.com/providers/Microsoft.PowerApps/environments?api-version=2016-11-01", "{}");
            var responseStringWithNames = await responseWithNames.Content.ReadAsStringAsync();
            var responseParsedWithNames = JsonUtility.GetJsonObjectFromJsonString(responseStringWithNames);

            var response = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Get, $"https://management.azure.com/providers/Microsoft.PowerApps/environments?api-version=2016-11-01&$filter=minimumAppPermission%20eq%20%27CanEdit%27&$expand=Permissions&_poll=true", "{}");
            var responseString = await response.Content.ReadAsStringAsync();
            var responseParsed = JsonUtility.GetJsonObjectFromJsonString(responseString);

            var objectToSerialize = new RootObject();
            objectToSerialize.environments = new List<Environment>{};

            foreach (var env in responseParsed["value"])
            {
                if (env["properties"]["permissions"]["CreateDatabase"] != null)
                {
                    foreach (var obj in responseParsedWithNames["value"])
                    {
                        if (obj["name"].ToString() == env["name"].ToString())
                        {
                            objectToSerialize.environments.Add(new Environment { id = obj["name"].ToString(), name = obj["properties"]["displayName"].ToString() });
                        }
                    }
                };
            }

            var responseToReturn = JsonUtility.GetJsonStringFromObject(objectToSerialize);

            return new ActionResponse(ActionStatus.Success, responseToReturn);
        }
    }

    public class Environment
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class RootObject
    {
        public List<Environment> environments { get; set; }
    }
}