using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class LogicAppTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task CreateConnectionToLogicApp()
        {
            var dataStore = await AAD.GetTokenWithDataStore();
            var result = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(result.IsSuccess);
            var responseBody = JObject.FromObject(result.Body);
            var subscriptionId = responseBody["value"][0];

            dataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Private);
            dataStore.AddToDataStore("SelectedResourceGroup", "testing");

            var locationResult = await TestHarness.ExecuteActionAsync("Microsoft-GetLocations", dataStore);
            Assert.IsTrue(locationResult.IsSuccess);
            var location = locationResult.Body.GetJObject()["value"][5];
            dataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);

            var response = TestHarness.ExecuteAction("Microsoft-CreateResourceGroup", dataStore);

            string[] connectors = new string[5] {"bingnews", "azureml", "azureblob", "sql", "cognitiveservicetextanalytics"};


            foreach (string connector in connectors)
            {
                dataStore.AddToDataStore("ConnectorName", connector);
                response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
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
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            //Image Cacher Logic App
            dataStore.AddToDataStore("DeploymentName", "LogicAppDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "unittesttrialbpst" + randomString);
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "false");
            dataStore.AddToDataStore("LogicAppName", "testname");
            dataStore.AddToDataStore("ConnectorName", "azureblob");
            dataStore.AddToDataStore("ConnectorName", "azureblob");
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
        }
    }
}
