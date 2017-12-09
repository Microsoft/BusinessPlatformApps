using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Rest;
using Microsoft.Azure.Management.MachineLearning.WebServices;
using Microsoft.Azure.Management.MachineLearning.CommitmentPlans;
using Microsoft.Azure.Management.MachineLearning.WebServices.Util;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class AzureMLTest
    {
        [TestMethod]
        public async Task CreateAMLWebService()
        {
            var dataStore = await TestManager.GetDataStore();

            //// Deploy AML WebService
            dataStore.AddToDataStore("DeploymentName", "AMLDeploymentTest");
            dataStore.AddToDataStore("WebServiceFile", "Service/AzureML/Experiments/TopicImagesWebService.json");
            dataStore.AddToDataStore("WebServiceName", "$run('TopicsImagesWebService' + this.MS.DataStore.getValue('uniqueId'))");
            dataStore.AddToDataStore("CommitmentPlan", "commitmentplan");
            dataStore.AddToDataStore("SelectedResourceGroup", "desktop-424ud00test");
            dataStore.AddToDataStore("StorageAccountName", "storage8kxdl9wil");
            dataStore.AddToDataStore("StorageAccountKey", "h1N4bYA7oQazUMF0PXiE1KjKboR0LoC4l9pOge7UTWGkAQee3sW4Jx3RHc+oMSz60n92UcyCD8VjvGreKeJ7CQ==");
            dataStore.AddToDataStore("IsRequestResponse", true);

            var response = TestManager.ExecuteAction("Microsoft-DeployAzureMLWebServiceFromFile", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestManager.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }


        [TestMethod]
        public async Task DeployAmlExperiment()
        {
            var dataStore = await TestManager.GetDataStore();
            ActionResponse response = null;
            //// Deploy AML WebService
            dataStore.AddToDataStore("DeploymentName", "test1");
            dataStore.AddToDataStore("StorageAccountName", "testmostorage123456789");
            dataStore.AddToDataStore("ExperimentName", "test1");
            dataStore.AddToDataStore("ExperimentJsonPath", "Service/AzureML/Experiments/TopicImagesExperiment.json");
            dataStore.AddToDataStore("WorkspaceName", "bingnewsworkspace01");

            response = TestManager.ExecuteAction("Microsoft-DeployAzureMLWorkspace", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureMLExperiment", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("ExperimentName", "test2");
            dataStore.AddToDataStore("ExperimentJsonPath", "Service/AzureML/Experiments/EntityRecognitionExperiment.json");

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureMLExperiment", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("ExperimentName", "test3");
            dataStore.AddToDataStore("ExperimentJsonPath", "Service/AzureML/Experiments/TopicsExperiment.json");

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureMLExperiment", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployAmlWebService()
        {
            var dataStore = await TestManager.GetDataStore();
            ActionResponse response = null;
            //// Deploy AML WebService
            dataStore.AddToDataStore("CommitmentPlan", "test1");
            dataStore.AddToDataStore("WebServiceFile", "Service/AzureML/Experiments/TopicsWebService.json");
            dataStore.AddToDataStore("WebServiceName", "test1");
            dataStore.AddToDataStore("storageAccountName", "storagezfx06s8x23");
            dataStore.AddToDataStore("storageAccountKey", "Q8oXtEVYO4kBT7rTokNFNe3fi9ufo/9C6gxmoHNxNRNGPTSpn/AOY1TFK7vtfAtRKyAqaSWYNqCIop4ry7sS6g==");
            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureMLWebServiceFromFile", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task GetWebServiceDefinition()
        {
            // This test will extract all the webservice definitions from AML priceplan
            // Only run when you need it

            var dataStore = await TestManager.GetDataStore();
            var azureToken = dataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = "c6a33fd3-e442-48a4-b82d-bcc4ad8a71d7";
            var resourceGroup = "Api-Default-West-US";//dataStore.GetValue("SelectedResourceGroup");

            ServiceClientCredentials creds = new TokenCredentials(azureToken);
            AzureMLWebServicesManagementClient client = new AzureMLWebServicesManagementClient(creds);
            AzureMLCommitmentPlansManagementClient commitmentClient = new AzureMLCommitmentPlansManagementClient(creds);
            client.SubscriptionId = subscription;
            commitmentClient.SubscriptionId = subscription;
            var webservices = await client.WebServices.ListByResourceGroupAsync(resourceGroup);
            foreach (var webserviceName in webservices)
            {
                var webservice = await client.WebServices.GetAsync(resourceGroup, webserviceName.Name);
                var str = ModelsSerializationUtil.GetAzureMLWebServiceDefinitionJsonFromObject(webservice);
                System.IO.File.WriteAllText(webserviceName.Name.Replace(".", "").Replace(":", "") + "FFTWebService.json", str);
            }
        }

    }
}
