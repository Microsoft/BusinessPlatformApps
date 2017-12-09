using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

using Microsoft.Deployment.Actions.AzureCustom.AzureToken;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Tests.Actions.TestHelpers
{
    [TestClass]
    public class TestManager
    {
        public static string RandomString = RandomGenerator.GetRandomLowerCaseCharacters(8);
        public static string ResourceGroup = Environment.MachineName.ToLower();

        private static CommonController Controller { get; set; }
        public static string TemplateName = "Microsoft-NewsTemplateTest";

        private static async Task<DataStore> GetDataStoreWithToken(bool force = false, Dictionary<string, string> extraTokens = null)
        {
            // Read from file DataStore
            if (File.Exists("datastore.json") && !force)
            {
                string filecontents = File.ReadAllText("datastore.json");
                var jsonObj = JsonConvert.DeserializeObject<DataStore>(filecontents);

                RefreshAzureToken token = new RefreshAzureToken();
                ActionRequest req = new ActionRequest()
                {
                    DataStore = jsonObj
                };

                try
                {
                    var intercept = await token.CanInterceptAsync(null, req);
                    if (intercept == InterceptorStatus.Intercept)
                    {
                        await TestManager.ExecuteActionAsync("Microsoft-RefreshAzureToken", jsonObj);
                        System.IO.File.WriteAllText("datastore.json", JsonUtility.GetJObjectFromObject(jsonObj).ToString());
                    }

                    return jsonObj;
                }
                catch (Exception)
                {
                    // Skip over error and try again
                }
            }

            // If not found or refresh failed prompt
            Credential.Load();
            var dataStore = await AAD.GetUserTokenFromPopup();

            if (extraTokens != null)
            {
                foreach (KeyValuePair<string, string> pair in extraTokens)
                {
                    var datastoreExtra = await AAD.GetUserTokenFromPopup(pair.Key); // see AAD case 'powerbi'
                    dataStore.AddToDataStore(pair.Value, datastoreExtra.GetJson(pair.Value)); // {PBIToken:val}
                    dataStore.AddToDataStore("code" + pair.Key, datastoreExtra.GetValue("code"));
                    dataStore.AddToDataStore("state" + pair.Key, datastoreExtra.GetValue("state"));
                    dataStore.AddToDataStore("sessionstate" + pair.Key, datastoreExtra.GetValue("session_state"));
                }
            }

            var subscriptionResult = await TestManager.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(subscriptionResult.IsSuccess);
            var subscriptionId = subscriptionResult.Body.GetJObject()["value"].SingleOrDefault(p => p["DisplayName"].ToString().StartsWith("PBI_"));
            dataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Public);

            var locationResult = await TestManager.ExecuteActionAsync("Microsoft-GetLocations", dataStore);
            var location = locationResult.Body.GetJObject()["value"][5];
            dataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);
            dataStore.AddToDataStore("SelectedResourceGroup", ResourceGroup);

            var resourceGroupResult = await TestManager.ExecuteActionAsync("Microsoft-CreateResourceGroup", dataStore);

            System.IO.File.WriteAllText("datastore.json", JsonUtility.GetJObjectFromObject(dataStore).ToString());
            return dataStore;
        }

        public static async Task<DataStore> GetDataStore(bool force = false, Dictionary<string, string> extraTokens = null)
        {
            var dataStore = await GetDataStoreWithToken(force, extraTokens);
            return dataStore;
        }

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            AppFactory factory = new AppFactory(true);
            CommonControllerModel model = new CommonControllerModel()
            {
                AppFactory = factory,
                AppRootFilePath = factory.AppPath,
                SiteCommonFilePath = factory.SiteCommonPath,
                ServiceRootFilePath = factory.SiteCommonPath + "../",
                Source = "TEST",
            };

            Controller = new CommonController(model);
            Credential.Load();
        }

        public static ActionResponse ExecuteAction(string actionName, DataStore datastore, string templateName = "Microsoft-NewsTemplateTest")
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = templateName;
            info.WebsiteRootUrl = "https://unittest";
            return Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore }).Result;
        }

        public static async Task<ActionResponse> ExecuteActionAsync(string actionName, DataStore datastore, string templateName = "Microsoft-NewsTemplateTest")
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = templateName;
            info.WebsiteRootUrl = "https://unittest";
            return await Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore });
        }

        public static async Task<bool> IsSuccessAsync(string actionName, DataStore datastore, string templateName = "Microsoft-NewsTemplateTest")
        {
            return (await ExecuteActionAsync(actionName, datastore, templateName)).IsSuccess;
        }
    }
}