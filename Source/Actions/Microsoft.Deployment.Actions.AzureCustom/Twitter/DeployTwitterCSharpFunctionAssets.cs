using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.AzureCustom.Twitter
{
    [Export(typeof(IAction))]
    public class DeployTwitterCSharpFunctionAssets : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = request.DataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation")["Name"].ToString();

            var sitename = request.DataStore.GetValue("SiteName");
            var sqlConnectionString = request.DataStore.GetValue("SqlConnectionString");

            var apiKey = request.DataStore.GetValue("AzureMLKey");
            var apiUrl = request.DataStore.GetValue("AzureMLUrl");

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);

            var functionCSharp = System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/Data/TweetFunctionCSharp.cs"));
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
            jsonRequest["files"]["run.csx"] = functionCSharp;
            string stringRequest = JsonUtility.GetJsonStringFromObject(jsonRequest); 

            var functionCreated = await client.ExecuteWebsiteAsync(HttpMethod.Put, sitename, "/api/functions/TweetProcessingFunction",
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
            obj.connectionStrings = new ExpandoObject[3];
            obj.connectionStrings[0] = new ExpandoObject();
            obj.connectionStrings[0].ConnectionString = sqlConnectionString;
            obj.connectionStrings[0].Name = "connectionString";
            obj.connectionStrings[0].Type = 2;
            obj.connectionStrings[1] = new ExpandoObject();
            obj.connectionStrings[1].ConnectionString = apiKey;
            obj.connectionStrings[1].Name = "apiKey";
            obj.connectionStrings[1].Type = 2;
            obj.connectionStrings[2] = new ExpandoObject();
            obj.connectionStrings[2].ConnectionString = apiUrl;
            obj.connectionStrings[2].Name = "webserviceUrl";
            obj.connectionStrings[2].Type = 2;
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
