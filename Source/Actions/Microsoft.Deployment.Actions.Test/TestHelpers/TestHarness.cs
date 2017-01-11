using System;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Test.TestHelpers
{
    [TestClass]
    public class TestHarness
    {
        public static string RandomCharacters = RandomGenerator.GetRandomLowerCaseCharacters(5);
        private static CommonController Controller { get; set; }
        public static string TemplateName = "TestApp";
        private static DataStore CommonDataStoreServicePrincipal = null;
        private static DataStore CommonDataStoreUserToken = null;
        private static string ResourceGroup = "UnitTest" + RandomGenerator.GetRandomLowerCaseCharacters(5);
        public static string CurrentDatabase = string.Empty;


        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            AppFactory factory = new AppFactory();
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

        [AssemblyCleanup()]
        public static async void AssemblyCleanup()
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                var deleteResourceGroupResult =
                    await TestHarness.ExecuteActionAsync("Microsoft-DeleteResourceGroup", GetCommonDataStore().Result);
            }

            if (!string.IsNullOrWhiteSpace(CurrentDatabase))
            {
                RemoveTempDB();
            }
        }


        public static ActionResponse ExecuteAction(string actionName, DataStore datastore)
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = TemplateName;
            info.WebsiteRootUrl = "https://unittest";
            return Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore }).Result;
        }

        public static async Task<ActionResponse> ExecuteActionAsync(string actionName, DataStore datastore)
        {
            UserInfo info = new UserInfo();
            info.ActionName = actionName;
            info.AppName = TemplateName;
            info.WebsiteRootUrl = "https://unittest";
            return await Controller.ExecuteAction(info, new ActionRequest() { DataStore = datastore });
        }

        public static async Task<DataStore> GetCommonDataStore()
        {
            if (CommonDataStoreServicePrincipal == null)
            {
                CommonDataStoreServicePrincipal = await SetUp(false);
            }

            DataStore store =
                JsonConvert.DeserializeObject<DataStore>(JsonUtility.GetJsonStringFromObject(CommonDataStoreServicePrincipal));
            return store;
        }

        public static async Task<DataStore> GetCommonDataStoreWithUserToken()
        {
            if (CommonDataStoreUserToken == null)
            {
                CommonDataStoreUserToken = await SetUp(true);
            }

            DataStore store =
                JsonConvert.DeserializeObject<DataStore>(JsonUtility.GetJsonStringFromObject(CommonDataStoreUserToken));
            return store;
        }

        public static async Task<DataStore> SetUp(bool getUserToken = false)
        {
            DataStore dataStore = null;
            if (getUserToken)
            {
                tempDataStore = null;
                Thread newThread = new Thread(GetUserTokenThreadSafe);
                newThread.SetApartmentState(ApartmentState.STA);
                newThread.Start();
                while (newThread.ThreadState != ThreadState.Stopped)
                {
                    await Task.Delay(3000);
                }
                dataStore = tempDataStore;
            }
            else
            {
                dataStore = AAD.GetTokenWithDataStore().Result;
            }
            
            dataStore = await SetupDatastore(dataStore);
            return dataStore;
        }

        private static DataStore tempDataStore = null;

        public static void GetUserTokenThreadSafe()
        {
            tempDataStore = AAD.GetUserTokenFromPopup().Result;
        }

        public static async Task<DataStore> GetCommonDataStoreWithSql()
        {
            var dataStore = await GetCommonDataStore();

            if (string.IsNullOrWhiteSpace(CurrentDatabase))
            {
                CreateTempDB();
            }

            var connString = GetSqlPagePayload(CurrentDatabase);
            dataStore.AddToDataStore("SqlConnectionString", connString);
            return dataStore;
        }

        public static async Task<DataStore> SetupDatastore(DataStore dataStore)
        {
            var subscriptionResult = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(subscriptionResult.IsSuccess);
            var subscriptionId =
                subscriptionResult.Body.GetJObject()["value"].SingleOrDefault(
                    p => p["DisplayName"].ToString() == "PBI_ECO (Paas) Richard's team");
            dataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Public);

            var locationResult = await TestHarness.ExecuteActionAsync("Microsoft-GetLocations", dataStore);
            Assert.IsTrue(locationResult.IsSuccess);
            var location = locationResult.Body.GetJObject()["value"][5];
            dataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);


            if (System.Diagnostics.Debugger.IsAttached)
            {
                ResourceGroup = Environment.MachineName;
            }

            dataStore.AddToDataStore("SelectedResourceGroup", ResourceGroup);


            if (!System.Diagnostics.Debugger.IsAttached)
            {
                var deleteResourceGroupResult = await TestHarness.ExecuteActionAsync("Microsoft-DeleteResourceGroup", dataStore);
            }

            var resourceGroupResult = await TestHarness.ExecuteActionAsync("Microsoft-CreateResourceGroup", dataStore);
            Assert.IsTrue(resourceGroupResult.IsSuccess);
            return dataStore;
        }

        public static void CreateTempDB()
        {
            CurrentDatabase = Guid.NewGuid().ToString();

            SqlCredentials creds = new SqlCredentials()
            {
                Server = Credential.Instance.Sql.Server,
                Username = Credential.Instance.Sql.Username,
                Password = Credential.Instance.Sql.Password,
                Authentication = SqlAuthentication.SQL,
                Database = "master"
            };

            string command = $"SET IMPLICIT_TRANSACTIONS OFF; " +
                             $"IF EXISTS (SELECT * FROM sys.databases WHERE name='{CurrentDatabase}') " +
                             $"DROP DATABASE [{CurrentDatabase}]; CREATE DATABASE [{CurrentDatabase}];";

            RunSqlCommandWithoutTransaction(creds, command);
            Credential.Instance.Sql.Database = CurrentDatabase;
        }

        public static void RemoveTempDB()
        {
            SqlCredentials creds = new SqlCredentials()
            {
                Server = Credential.Instance.Sql.Server,
                Username = Credential.Instance.Sql.Username,
                Password = Credential.Instance.Sql.Password,
                Authentication = SqlAuthentication.SQL,
                Database = "master"
            };
            
            string command = $"SET IMPLICIT_TRANSACTIONS OFF; " +
                            $"IF EXISTS (SELECT * FROM sys.databases WHERE name='{CurrentDatabase}') " +
                            $"DROP DATABASE [{CurrentDatabase}];";

            RunSqlCommandWithoutTransaction(creds, command);
        }

        public  static string GetSqlPagePayload(string database)
        {
            var dataStore = new DataStore();

            dynamic sqlPayload = new ExpandoObject();
            sqlPayload.SqlCredentials = new ExpandoObject();
            sqlPayload.SqlCredentials.Server = Credential.Instance.Sql.Server;
            sqlPayload.SqlCredentials.AuthType = "azuresql";
            sqlPayload.SqlCredentials.User = Credential.Instance.Sql.Username;
            sqlPayload.SqlCredentials.Password = Credential.Instance.Sql.Password;
            sqlPayload.SqlCredentials.Database = database;

            dataStore.AddObjectDataStore("SqlCredentials", JsonUtility.GetJObjectFromObject(sqlPayload), DataStoreType.Any);

            ActionResponse sqlResponse = TestHarness.ExecuteAction("Microsoft-GetSqlConnectionString", dataStore);
            Assert.IsTrue(sqlResponse.Status == ActionStatus.Success);

            return (sqlResponse.Body as JObject)["value"].ToString();
        }

        public static void RunSqlCommandWithoutTransaction(SqlCredentials creds, string commandText)
        {
            var connString = SqlUtility.GetConnectionString(creds).Replace("Connect Timeout=15", "Connect Timeout=60");
            using (var cn = new SqlConnection(connString))
            {
                cn.Open();
                using (var command = cn.CreateCommand())
                {
                    command.CommandTimeout = 60;
                    command.CommandText = commandText;
                    command.Transaction = null;
                    command.ExecuteNonQuery();
                }
                cn.Close();
            }
        }
    }
}
