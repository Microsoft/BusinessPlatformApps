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
            string id = RandomGenerator.GetRandomCharacters();
            System.Diagnostics.Debug.WriteLine($"id: {id}");
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("DeploymentName", $"LanceSADeployment-{id}");
            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/StreamAnalytics.json");
            var payload = new JObject();
            payload.Add("name", "POC-StreamAnalyticsJob2");
            //payload.Add("name", $"LancesStreamAnalyticsJob-{id}");
            dataStore.AddToDataStore("AzureArmParameters", payload);

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
            dataStore.AddToDataStore("jobName", "POC-StreamAnalyticsJob");
            SetupDataStoreWithEventHubKey(dataStore); // key "primaryKey" maps to corresponding event hub's primary policy key
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetStreamAnalyticsInputAsEventHub", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetStreamAnalyticsQueryTest()
        {
            // Tests the Action to define the query of a Stream Analytics job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("inputAlias", "POC-input");
            dataStore.AddToDataStore("outputAlias", "POC-output");
            // Since we technically update the default query instead of creaitng one, use the deafult 
            // transformation name which is "Transformation"
            dataStore.AddToDataStore("transformationName", "Transformation");
            dataStore.AddToDataStore("jobName", "POC-StreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetStreamAnalyticsQuery", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task StartStreamAnalyticsJobTest()
        {
            // Tests the Action to start a Stream Analytics job
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("jobName", "POC-StreamAnalyticsJob");
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
            dataStore.AddToDataStore("jobName", "LancesStreamAnalyticsJob");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetOutputAsPBI", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task SetOutputAsSQLTest()
        {
            // Tests the Action to output from Stream Analytics to SQL
            var dataStore = await TestManager.GetDataStore(true);
            dataStore.AddToDataStore("jobName", "POC-StreamAnalyticsJob");
            dataStore.AddToDataStore("serverName", "pbisttest.database.windows.net");
            dataStore.AddToDataStore("dbName", "LancesSQLDB");
            dataStore.AddToDataStore("username", "pbiadmin@pbisttest");
            dataStore.AddToDataStore("password", TestHelpers.Credential.Instance.Sql.Password);
            dataStore.AddToDataStore("tableName", "eventHubSQL");
            dataStore.AddToDataStore("outputAlias", "POC-sqloutput");
            var response = await TestManager.ExecuteActionAsync("Microsoft-SetOutputAsSQL", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
