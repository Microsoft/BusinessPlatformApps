using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployAzureFunctionAssets : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var functionFileName = request.DataStore.GetValue("FunctionFileName");
            var functionName = request.DataStore.GetValue("FunctionName");

            var sitename = request.DataStore.GetValue("SiteName");

            List<string> appSettings = new List<string>();

            if (request.DataStore.GetJson("AppSettingKeys") != null && !string.IsNullOrEmpty(request.DataStore.GetJson("AppSettingKeys")[0].ToString()))
            {
                foreach (var item in request.DataStore.GetJson("AppSettingKeys"))
                {
                    string key = (string)item;
                    appSettings.Add(key);
                }
            }


            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            var function = System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, $"Service/Data/{functionFileName}"));
            var jsonBody =
                "{\"files\":{\"run.csx\":\"test\"},\"config\":" +
                "{\"" +
                "bindings\":" +
                "[" +
                    "{\"name\":\"req\"," +
                    "\"type\":\"httpTrigger\"," +
                    "\"direction\":\"in\"," +
                    "\"webHookType\":\"genericJson\"," +
                    "\"scriptFile\":\"run.csx\"" +
                    "}" +
                 "]," +
                 "\"disabled\":false}}";

            JObject jsonRequest = JsonUtility.GetJObjectFromJsonString(jsonBody);
            jsonRequest["files"]["run.csx"] = function;
            string stringRequest = JsonUtility.GetJsonStringFromObject(jsonRequest);

            var functionCreated = await client.ExecuteWebsiteAsync(HttpMethod.Put, sitename, $"/api/functions/{functionName}",
            stringRequest);

            string response = await functionCreated.Content.ReadAsStringAsync();
            if (!functionCreated.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(response),
                    null, DefaultErrorCodes.DefaultErrorCode, "Error creating function");
            }

            dynamic obj = new ExpandoObject();
            obj.subscriptionId = subscription;
            obj.siteId = new ExpandoObject();
            obj.siteId.Name = sitename;
            obj.siteId.ResourceGroup = resourceGroup;
            obj.connectionStrings = new ExpandoObject[appSettings.Count];
            if (appSettings.Count != 0)
            {
                for (int i = 0; i < appSettings.Count; i++)
                {
                    obj.connectionStrings[i] = new ExpandoObject();
                    obj.connectionStrings[i].ConnectionString = appSettings[i];
                    obj.connectionStrings[i].Name = "connectionString" + i.ToString();
                    obj.connectionStrings[i].Type = 2;
                }
            }
            obj.location = location;

            var appSettingCreated = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, @"https://web1.appsvcux.ext.azure.com/websites/api/Websites/UpdateConfigConnectionStrings",
            JsonUtility.GetJsonStringFromObject(obj));
            response = await appSettingCreated.Content.ReadAsStringAsync();
            if (!appSettingCreated.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(response),
                    null, DefaultErrorCodes.DefaultErrorCode, "Error creating appsetting");
            }

            var getFunction = await client.ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Get,
            $"/providers/Microsoft.Web/sites/{sitename}", "2015-08-01", string.Empty);
            response = await getFunction.Content.ReadAsStringAsync();

            if (!getFunction.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(response),
                    null, DefaultErrorCodes.DefaultErrorCode, "Error creating appsetting");
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}
