using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class TwiiterTemplateTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task TwitterTemplateDeploymentAS()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            #region DeploySQLScripts

             // Deploy SQL Scripts
            dataStore.AddToDataStore("SqlConnectionString", TestHarness.GetSqlPagePayload("ssas2"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/LogicApps");
            var response = await TestHarness.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);


            // TEMP
            // CHANGE PASSWORD
            dataStore.AddToDataStore("ASServerName", "asserver5");
            dataStore.AddToDataStore("ASLocation", "westcentralus");
            dataStore.AddToDataStore("ASSku", "D1");
            dataStore.AddToDataStore("ASAdminPassword", "Uthman77777");
            dataStore.AddToDataStore("xmlaFilePath", "Service/SSAS/twitter.xmla");
            dataStore.AddToDataStore("ASDatabase", "TwitterModel");

            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureAnalysisServices", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);

            response = await TestHarness.ExecuteActionAsync("Microsoft-ValidateConnectionToAS", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);



            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureASModel", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "unittestfunction5");
            dataStore.AddToDataStore("RepoUrl", "https://github.com/MohaaliMicrosoft/AnalysisServicesRefresh");
            dataStore.AddToDataStore("sku", "Standard");

            response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);


            //// Deploy ConnectionStrings
            dynamic payload = new ExpandoObject();
            payload.connStringAS = dataStore.GetValue("ASConnectionString");
            payload.connStringSql = dataStore.GetValue("SqlConnectionString");
            payload.schema = "[pbist_twitter]";
            payload.databaseAS = "TwitterModel";

            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("AppSettingKeys", payload);
            response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunctionConnectionStrings", dataStore);
            Assert.IsTrue(response.IsSuccess);
            #endregion
        }
    }
}

