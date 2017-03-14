using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class AzureAnalysisServicesTest
    {
        [TestMethod]
        public async Task OlapTest()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = new DataStore();

            dataStore.AddToDataStore("ASServerUrl", "asazure://westus.asazure.windows.net/ssastestserver2");
            dataStore.AddToDataStore("ASAdmin", "");
            dataStore.AddToDataStore("ASAdminPassword", "3213");

            var response = await TestManager.ExecuteActionAsync("Microsoft-ValidateConnectionToAS", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployASModelTest()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore();

            // Deploy Twitter Database Scripts
            dataStore.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("ssas"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/LogicApps");

            var response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);

            dataStore.AddToDataStore("ASServerName", "asservermo");
            dataStore.AddToDataStore("ASLocation", "westcentralus");
            dataStore.AddToDataStore("ASSku", "D1");

            dataStore.AddToDataStore("ASAdminPassword", "Uthman77777");
            dataStore.AddToDataStore("xmlaFilePath", "Service/SSAS/twitter.xmla");
            dataStore.AddToDataStore("ASDatabase", "testdb");

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureAnalysisServices", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-ValidateConnectionToAS", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureASModel", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.IsSuccess);

        }
    }
}
