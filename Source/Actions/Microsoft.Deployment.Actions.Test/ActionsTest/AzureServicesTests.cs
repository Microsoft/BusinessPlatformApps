//using System.Threading.Tasks;
//using Microsoft.AnalysisServices.Tabular;
//using Microsoft.Deployment.Actions.Test.TestHelpers;
//using Microsoft.Deployment.Common.ActionModel;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace Microsoft.Deployment.Actions.Test.ActionsTest
//{
//    [TestClass]
//    public class AzureServicesTests
//    {
//        [TestMethod]
//        public async Task DeployAzureAnalysisServices()
//        {
//            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
//            dataStore.AddToDataStore("ASServerName", "asserver");
//            dataStore.AddToDataStore("ASLocation", "westcentralus");
//            dataStore.AddToDataStore("ASSku", "D1");

//            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureAnalysisServices", dataStore);
//            Assert.IsTrue(response.IsSuccess);

//            Assert.IsTrue(dataStore.KeyExists("ASServerUrl"));
//        }

//        [TestMethod]
//        public async Task CreateAndConnectToAzureAndDeploy()
//        {
//            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

//            // Deploy Twitter Database Scripts
//            dataStore.AddToDataStore("SqlConnectionString", TestHarness.GetSqlPagePayload("test"));
//            dataStore.AddToDataStore("SqlServerIndex", "0");
//            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/LogicApps");

//            var response = await TestHarness.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-TwitterTemplate");
//            Assert.IsTrue(response.Status == ActionStatus.Success);

//            dataStore.AddToDataStore("ASServerName", "asserver2");
//            dataStore.AddToDataStore("ASLocation", "westcentralus");
//            dataStore.AddToDataStore("ASSku", "D1");

//            dataStore.AddToDataStore("ASAdminPassword", "Required");
//            dataStore.AddToDataStore("xmlaFilePath", "Service/SSAS/twitter.xmla");
//            dataStore.AddToDataStore("ASDatabase", "testdb");

//            response = TestHarness.ExecuteAction("Microsoft-DeployAzureAnalysisServices", dataStore);
//            Assert.IsTrue(response.IsSuccess);

//            response = TestHarness.ExecuteAction("Microsoft-DeployAzureASModel", dataStore);
//            Assert.IsTrue(response.IsSuccess);
//        }

//        [TestMethod]
//        public async Task DeployModelToAzureAnalysisServices()
//        {
            
//        }

//        [TestMethod]
//        public async Task ProcessModel()
//        {
            
//        }

//    }
//}
