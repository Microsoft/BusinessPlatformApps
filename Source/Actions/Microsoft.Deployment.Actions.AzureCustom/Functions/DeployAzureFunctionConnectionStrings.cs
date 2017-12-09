using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.Common
{
    [Export(typeof(IAction))]
    public class DeployAzureFunctionConnectionStrings : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var sitename = request.DataStore.GetValue("FunctionName");

            List<KeyValuePair<string,string>> appSettings = new List<KeyValuePair<string, string>>();

            if (request.DataStore.GetJson("AppSettingKeys") != null )
            {
                foreach (var item in request.DataStore.GetJson("AppSettingKeys"))
                {
                    string key = item.Path.Split('.').Last();
                    string value = (string)item;
                    appSettings.Add(new KeyValuePair<string, string>(key,value));
                }
            }

            AzureHttpClient client = new AzureHttpClient(azureToken, subscription, resourceGroup);
            dynamic obj = new ExpandoObject();
            obj.subscriptionId = subscription;
            obj.siteId = new ExpandoObject();
            obj.siteId.Name = sitename;
            obj.siteId.ResourceGroup = resourceGroup;
            obj.connectionStrings = new ExpandoObject[appSettings.Count];
            obj.location = location;
            if (appSettings.Count != 0)
            {
                for (int i = 0; i < appSettings.Count; i++)
                {
                    obj.connectionStrings[i] = new ExpandoObject();
                    obj.connectionStrings[i].ConnectionString = appSettings[i].Value;
                    obj.connectionStrings[i].Name = appSettings[i].Key;
                    obj.connectionStrings[i].Type = 2;
                }
            }

            var appSettingCreated = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, @"https://web1.appsvcux.ext.azure.com/websites/api/Websites/UpdateConfigConnectionStrings",
            JsonUtility.GetJsonStringFromObject(obj));
            var response = await appSettingCreated.Content.ReadAsStringAsync();
            if (!appSettingCreated.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(response), null, DefaultErrorCodes.DefaultErrorCode, "Error creating appsetting");
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}