using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class CogntiveServiceTests
    {
        [TestMethod]
        public async Task DeployCognitiveServiceTextTest()
        {
            var dataStore = await TestHarness.GetCommonDataStore();

            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService");
            dataStore.AddToDataStore("CognitiveSkuName", "F0");
            dataStore.AddToDataStore("DeploymentName", "deployment");

           var response = TestHarness.ExecuteAction("Microsoft-DeployCognitiveServiceText", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            response = TestHarness.ExecuteAction("Microsoft-GetCognitiveServiceKeys", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            dataStore.AddObjectDataStore(response.Body.GetJObject(), DataStoreType.Private);
            Assert.IsTrue(dataStore.GetValue("CognitiveServiceKey") != null);
        }

        [TestMethod]
        public async Task DeployCognitiveServiceTest()
        {
            var dataStore = await AAD.GetTokenWithDataStore();
            var result = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(result.IsSuccess);
            var responseBody = JObject.FromObject(result.Body);
            var subscriptionId = responseBody["value"][0];

            dataStore.AddToDataStore("DeploymentName", "CongitiveServiceDeploy", DataStoreType.Private);
            dataStore.AddToDataStore("SelectedSubscription", subscriptionId, DataStoreType.Private);
            dataStore.AddToDataStore("SelectedResourceGroup", "testing");
            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService2");
            //dataStore.AddToDataStore("CognitiveServiceType", "TextAnalytics");
            dataStore.AddToDataStore("CognitiveServiceType", "Bing.Search");

            dataStore.AddToDataStore("CognitiveSkuName", "S1");
            var locationResult = await TestHarness.ExecuteActionAsync("Microsoft-GetLocations", dataStore);
            Assert.IsTrue(locationResult.IsSuccess);
            var location = locationResult.Body.GetJObject()["value"][5];
            dataStore.AddToDataStore("SelectedLocation", location, DataStoreType.Public);

            var response = TestHarness.ExecuteAction("Microsoft-CreateResourceGroup", dataStore);
            response = TestHarness.ExecuteAction("Microsoft-DeployCognitiveService", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            //if (response.Status == ActionStatus.Success)
            //{
            //    response = TestHarness.ExecuteAction("Microsoft-DeleteResourceGroup", dataStore);
            //    Assert.IsTrue(response.Status == ActionStatus.Success);
            //}
        }

        [TestMethod]
        public async Task GetCognitiveServiceKeys()
        {
            var dataStore = await AAD.GetTokenWithDataStore();
            var result = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
            Assert.IsTrue(result.IsSuccess);
            var responseBody = JObject.FromObject(result.Body);
            var subscriptionId = responseBody["value"][0];

            dataStore.AddToDataStore("Azure", "SelectedResourceGroup", "testing");
            dataStore.AddToDataStore("CognitiveService", "CognitiveServiceName", "TestCognitiveService2");

            var response = TestHarness.ExecuteAction("Microsoft-GetCognitiveKey", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }


        //[TestMethod]
        //public async Task TestValidCognitiveKey()
        //{
        //    var dataStore = await AAD.GetTokenWithDataStore();
        //    var result = await TestHarness.ExecuteActionAsync("Microsoft-GetAzureSubscriptions", dataStore);
        //    Assert.IsTrue(result.IsSuccess);
        //    var responseBody = JObject.FromObject(result.Body);
        //    var subscriptionId = responseBody["value"][0];

        //    string CognitiveServiceKey = "d917a971e5ef42b3b817608580c77cee";
        //    dataStore.AddToDataStore("CognitiveServiceKey", CognitiveServiceKey);
        //    var response = TestService.Instance.ExecuteAction("Microsoft-ValidateCognitiveKey", payload);

        //    Assert.IsTrue(response.Status == ActionStatus.Success);
        //}


        //[TestMethod]
        //public void TestInvalidCognitiveKey()
        //{
        //    var dataStore = GetDataStoreWithTokenSubscriptionLocation();
        //    dataStore.AddItemToDataStore("CognitiveService", "CognitiveServiceKey", "1223");
        //    var payload = dataStore.GetDataStore();
        //    var response = TestService.Instance.ExecuteAction("Microsoft-ValidateCognitiveKey", payload);
        //    Assert.IsTrue(response.Status == ActionStatus.FailureExpected);
        //}


    }
}
