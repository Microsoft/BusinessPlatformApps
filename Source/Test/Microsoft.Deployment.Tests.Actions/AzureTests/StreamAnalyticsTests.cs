using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class ConfigureStreamAnalyticsTest
    {
        [TestMethod]
        public async Task CreateStreamAnalyticsJobTest()
        {
            // Tests the Action to create a Stream Analytics job (with no input, default query, and no output)
            var dataStore = await TestManager.GetDataStore(true);
            var deployArmResult = await TestManager.ExecuteActionAsync(
                "Microsoft-CreateStreamAnalyticsJob", dataStore, "Microsoft-ActivityLogTemplate");
            Assert.IsTrue(deployArmResult.IsSuccess);
        }

        public void SetupDataStoreWithEventHubKey(DataStore dataStore)
        {
            // Helper method to add the Event Hub policy primary key (required to set SA input as EH) to the dataStore 
            var response = TestManager.ExecuteActionAsync("Microsoft-GetEventHubPrimaryKey", dataStore).Result;
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetStreamAnalyticsInputTest()
        {
            // Tests the Action to set Event Hub as the input for a Stream Analyitcs job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("namespace", "POC-Namespace3");
            dataStore.AddToDataStore("inputAlias", "POC-input2");
            //dataStore.AddToDataStore("jobName", "LancesStreamAnalyticsJob");
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            SetupDataStoreWithEventHubKey(dataStore); // key "primaryKey" maps to corresponding event hub's primary policy key
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetStreamAnalyticsInputAsEventHub", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetStreamAnalyticsFunctionTest()
        {
            // Tests the Action to set Event Hub as the input for a Stream Analyitcs job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetStreamAnalyticsJSONFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetStreamAnalyticsQueryTest()
        {
            // Tests the Action to define the query of a Stream Analytics job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("inputAlias", "POC-input");
            dataStore.AddToDataStore("outputAlias", "POC-output");
            // Since we technically update the default query instead of creating one, use the default 
            // transformation name which is "Transformation"
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetStreamAnalyticsQuery", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task StartStreamAnalyticsJobTest()
        {
            // Tests the Action to start a Stream Analytics job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-StartStreamAnalyticsJob", dataStore);
            Assert.IsTrue(response.IsSuccess);

        }

        [TestMethod]
        public async Task SetOutputAsPBITest()
        {
            // Tests the Action to output from Stream Analytics to Power BI
            Dictionary<string, string> extraTokens = new Dictionary<string, string>();
            extraTokens.Add("powerbi", "PBIToken"); // request PBI token 
            var dataStore = await TestManager.GetDataStore(true, extraTokens);
            dataStore.AddToDataStore("SAjob", "LancesStreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetOutputAsPBI", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetServiceHealthOutputTest()
        {
            // Tests the Action to output from Stream Analytics to SQL
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            dataStore.AddToDataStore("Server", TestHelpers.Credential.Instance.Sql.Server);
            dataStore.AddToDataStore("Database", "LancesSQLDB");
            dataStore.AddToDataStore("Username", TestHelpers.Credential.Instance.Sql.Username + "@pbisttest");
            dataStore.AddToDataStore("Password", TestHelpers.Credential.Instance.Sql.Password);
            dataStore.AddToDataStore("ServiceHealthTable", "eventHubSQL");
            dataStore.AddToDataStore("outputAlias", "ServiceHealth");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetServiceHealthOutput", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetAdministrativeOutputTest()
        {
            // Tests the Action to output from Stream Analytics to SQL
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("SAjob", "POC-StreamAnalyticsJob");
            dataStore.AddToDataStore("Server", TestHelpers.Credential.Instance.Sql.Server);
            dataStore.AddToDataStore("Database", "LancesSQLDB");
            dataStore.AddToDataStore("Username", TestHelpers.Credential.Instance.Sql.Username + "@pbisttest");
            dataStore.AddToDataStore("Password", TestHelpers.Credential.Instance.Sql.Password);
            dataStore.AddToDataStore("AdministrativeTable", "adminTable");
            dataStore.AddToDataStore("outputAlias", "AdministrativeOutput");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetAdministrativeOutput", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
