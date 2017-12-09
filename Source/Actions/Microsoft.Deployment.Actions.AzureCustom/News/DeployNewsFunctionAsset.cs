using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployNewsFunctionAsset : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var apiKey = request.DataStore.GetValue("apiKey");
            var subscriptionKey = request.DataStore.GetValue("subscriptionKey");
            string connectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");
            var sitename = request.DataStore.GetValue("FunctionName");

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

            dynamic obj = new ExpandoObject();
            obj.subscriptionId = subscription;
            obj.siteId = new ExpandoObject();
            obj.siteId.Name = sitename;
            obj.siteId.ResourceGroup = resourceGroup;
            obj.connectionStrings = new ExpandoObject[3];
            obj.connectionStrings[0] = new ExpandoObject();
            obj.connectionStrings[0].ConnectionString = apiKey;
            obj.connectionStrings[0].Name = "apiKey";
            obj.connectionStrings[0].Type = 2;
            obj.connectionStrings[1] = new ExpandoObject();
            obj.connectionStrings[1].ConnectionString = subscriptionKey;
            obj.connectionStrings[1].Name = "subscriptionKey";
            obj.connectionStrings[1].Type = 2;

            obj.connectionStrings[2] = new ExpandoObject();
            obj.connectionStrings[2].ConnectionString = connectionString;
            obj.connectionStrings[2].Name = "ConnectionString";
            obj.connectionStrings[2].Type = 2;
            obj.location = location;


            var appSettingCreated = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, @"https://web1.appsvcux.ext.azure.com/websites/api/Websites/UpdateConfigConnectionStrings",
            JsonUtility.GetJsonStringFromObject(obj));
            string response = await appSettingCreated.Content.ReadAsStringAsync();
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

            return new ActionResponse(ActionStatus.Success);
        }
    }
}