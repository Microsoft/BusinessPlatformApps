using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class LogicAppTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task CreateConnectionToLogicApp()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            //string[] connectors = new string[5] { "bingnews", "azureml", "azureblob", "sql", "cognitiveservicestextanalytics" };

            string[] connectors = new string[1] { "cognitiveservicestextanalytics" };
            foreach (string connector in connectors)
            {
                dataStore.AddToDataStore("ConnectorName", connector);
                var response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
                Assert.IsTrue(response.Status == ActionStatus.Success);
            }
        }

        [TestMethod]
        public async Task UpdateLogicApp()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("ConnectorName", "azureblob");
            dynamic payload = new ExpandoObject();
            payload.accountName = "cacheimages";
            payload.accessKey = "bsDCAU00sSqE48QIg+7cXKNhJbG7/0HnMzzl6nN0Y6L2pJoSTFvpHdKlpQPjjayKtks/IDeU2ep1ONPZh7UAKg==";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "testname");
            var response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }


        [TestMethod]
        public async Task CreateLogicApp()
        {
            //Manual steps - deploy function + deploy cognitive services
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            //Image Cache Logic App
            dataStore.AddToDataStore("DeploymentName", "LogicAppDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "unittesttrialbpst" + randomString);
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "false");
            dataStore.AddToDataStore("LogicAppName", "testname");
            dataStore.AddToDataStore("SearchQuery", "microsoft");
            dataStore.AddToDataStore("ConnectorName", "azureblob");
            dataStore.AddToDataStore("ConnectorDisplayName", "azureblob");
            dataStore.AddToDataStore("SiteName", "unituestrialbpstxskjf");
            dataStore.AddToDataStore("ImageCacheLogicApp", "testname");

            dynamic payload = new ExpandoObject();
            payload.accountName = "cacheimages";
            payload.accessKey = "bsDCAU00sSqE48QIg+7cXKNhJbG7/0HnMzzl6nN0Y6L2pJoSTFvpHdKlpQPjjayKtks/IDeU2ep1ONPZh7UAKg==";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);

            var response = TestHarness.ExecuteAction("Microsoft-CreateAzureStorageAccount", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-DeployImageCachingLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //NewsTemplateLogicApp
            dataStore.AddToDataStore("ConnectorName", "bingnews");
            payload = new ExpandoObject();
            payload.apiKey = "a1a17649b8784afd9219fdcf3f945552";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "BingNews");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("ConnectorName", "cognitiveservicestextanalytics");
            payload = new ExpandoObject();
            payload.apiKey = "488546b19ba040179eaaf172f19196cf";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "TextAnalytics");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("ConnectorName", "sql");
            payload = new ExpandoObject();
            payload.authType = "windows";
            payload.database = "testruns";
            payload.password = "Billing.26";
            payload.server = "pbist.database.windows.net";
            payload.username = "pbiadmin";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "InsertIngestTimeStamp");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("LogicAppName", "testname2");
            response = TestHarness.ExecuteAction("Microsoft-DeployNewsTemplateLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);


        }

        [TestMethod]
        public async Task CreateLogicAppAzureML()
        {
            //Manual steps - create Azure ML experiments + deploy SQL scripts
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
    

            dataStore.AddToDataStore("DeploymentName", "LogicAppDeploymentTest");
            dataStore.AddToDataStore("LogicAppName", "testname");
            dataStore.AddToDataStore("Exp1", "TopicApiKey", "VB4mYM5WD0EzEFvJ8z8sIpei4Y85oiIdGAT5z/G/+YVfWSu8ISvECZ8KalKji6a6AEkxoFDpNrhsrMUq0mkJjA==");
            dataStore.AddToDataStore("Exp2", "ImagesApiKey", "TM1oZ3taPmRiR0V6wxEkDYAQaRip9JxJ+HQhyo/9T9VcwSpbobye+jNPDyMayN+y8u+PQVFv1gbH1sIhft0uqQ==");
            dataStore.AddToDataStore("Exp3", "EntityApiKey", "VYDEekiPGZDSFTnjgURi8ik+hIHh4AwJTT7RvdH9FUL0MsS/NpAgtRyUkQROuHG+FTIGohhoLYfwWxhjZgBJSg==");

            dataStore.AddToDataStore("Exp1", "TopicApiUrl", "https://ussouthcentral.services.azureml.net/workspaces/0a5545bf61b948c5b04684eadae10e09/services/2b67d0d308e646b2b8a0f9d7d934b5c7/jobs?api-version=2.0");
            dataStore.AddToDataStore("Exp2", "ImagesApiUrl", "https://ussouthcentral.services.azureml.net/workspaces/0a5545bf61b948c5b04684eadae10e09/services/f321bb3ab3624173a7c0de239957e6dd/jobs?api-version=2.0");
            dataStore.AddToDataStore("Exp3", "EntityApiUrl", "https://ussouthcentral.services.azureml.net/workspaces/0a5545bf61b948c5b04684eadae10e09/services/02e09e59cddd4cbe8027a5b93974ad0a/jobs?api-version=2.0");

            //Create AzureML Connector
            dataStore.AddToDataStore("ConnectorName", "azureml");
            dataStore.AddToDataStore("ConnectorDisplayName", "azureml");
            dynamic payload = new ExpandoObject();
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            var response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Create SQL Connector
            dataStore.AddToDataStore("ConnectorName", "sql");
            payload = new ExpandoObject();
            payload.authType = "windows";
            payload.database = "testruns";
            payload.password = "Billing.26";
            payload.server = "pbist.database.windows.net";
            payload.username = "pbiadmin";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "SQLConnector");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Create Logic App
            response = TestHarness.ExecuteAction("Microsoft-DeployAzureMLSchedulerLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
