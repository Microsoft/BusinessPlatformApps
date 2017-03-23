using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.AzureCustom.AzureToken;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Tests.Actions.TestHelpers
{
    [TestClass]
    public class TestManager
    {
        public static string RandomString = RandomGenerator.GetRandomLowerCaseCharacters(8);
        public static string ResourceGroup = Environment.MachineName.ToLower() + "test";

        private static CommonController Controller { get; set; }
        public static string TemplateName = "Microsoft-NewsTemplateTest";

        private static async Task<DataStore> GetDataStoreWithToken(bool force = false)
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
            var subscriptionResult = await TestManager.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(subscriptionResult.IsSuccess);
            var subscriptionId = subscriptionResult.Body.GetJObject()["value"].SingleOrDefault(p => p["DisplayName"].ToString().StartsWith("PBI_"));
            dataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Public);

            var locationResult = await TestManager.ExecuteActionAsync("Microsoft-GetLocations", dataStore);
            var location = locationResult.Body.GetJObject()["value"][5];
            dataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);
            dataStore.AddToDataStore("SelectedResourceGroup", ResourceGroup);

            var resourceGroupResult = await TestManager.ExecuteActionAsync("Microsoft-CreateResourceGroup", dataStore);
            Assert.IsTrue(resourceGroupResult.IsSuccess);

            System.IO.File.WriteAllText("datastore.json", JsonUtility.GetJObjectFromObject(dataStore).ToString());
            return dataStore;
        }

        public static async Task<DataStore> GetDataStore(bool force = false)
        {
            var dataStore = await GetDataStoreWithToken(force);
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

        //[AssemblyCleanup()]
        //public static async void AssemblyCleanup()
        //{
        //}

        public static ActionResponse ExecuteAction(string actionName, DataStore datastore)
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = TemplateName;
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
    }
}
