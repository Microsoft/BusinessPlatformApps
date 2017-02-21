using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
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
    public class AzureFunctionsTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task DeployAzureFunction()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("Name", "unituestrialbpst" + randomString);

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }


        [TestMethod]
        public async Task DeployTwitterAzureFunction()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "unituestrialbpst" + randomString);

            var response = TestHarness.ExecuteAction("Microsoft-DeployTwitterFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployAzureFunctionAssets()
        {
            await this.DeployAzureFunction();
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "UnitTestTrialbpst" + randomString);

            dataStore.AddToDataStore("FunctionName", "ASCIICleanser");
            dataStore.AddToDataStore("FunctionFileName", "ASCIICleanser.csx");

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunctionAssets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            dataStore.AddToDataStore("FunctionName", "StringUtilities");
            dataStore.AddToDataStore("FunctionFileName", "StringUtilities.csx");

            response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunctionAssets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

        }

        [TestMethod]
        public async Task DeployTwitterCSharpFunctionAssetsTest()
        {
            await this.DeployAzureFunction();
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("SiteName", "UnitTestTrialbpst" + randomString);

            dataStore.AddToDataStore("FunctionName", "TestA");
            dataStore.AddToDataStore("FunctionFileName", "TweetFunctionCSharp.cs");

            var response = TestHarness.ExecuteAction("Microsoft-DeployTwitterCSharpFunctionAssets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

        }

        [TestMethod]
        public async Task DeployFunctionStaticAppPlan()
        {
            var dataStore = await TestHarness.GetCommonDataStore();
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "unittestfunction" + TestHarness.RandomCharacters);
            dataStore.AddToDataStore("sku", "Standard");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/juluczni/AzureFunctionsNewsTemplate");

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}

