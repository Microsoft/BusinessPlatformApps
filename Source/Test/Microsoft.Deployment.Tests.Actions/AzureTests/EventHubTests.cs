using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Tests.Actions.AzureTests
{
    [TestClass]
    public class EventHubTests
    {
        [TestMethod]
        public async Task CreateEventHubNameSpaceTest()
        {
            // Tests the Action to create an Event Hub Namespace
            var dataStore = await TestManager.GetDataStore(true);

            var deployArmResult = await TestManager.ExecuteActionAsync(
                "Microsoft-CreateEventHubNameSpace", dataStore, "Microsoft-ActivityLogTemplate");
        
            //var deployArmResult = await TestManager.ExecuteActionAsync(
            //    "Microsoft-DeployAzureArmTemplate", dataStore, "Microsoft-ActivityLogTemplate");
            Assert.IsTrue(deployArmResult.IsSuccess);
        }

        [TestMethod]
        public async Task ExportActivityLogToEventHubTest()
        {
            // Tests the Action to export an Activity Log to Event Hub
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("namespaceName", "POC-Namespace3");
            var response = await TestManager.ExecuteActionAsync("Microsoft-ExportActivityLogToEventHub", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task GetEventHubPrimaryKeyTest()
        {
            // Tests the Action to obtain the primary policy key for an Event Hub
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("namespace", "POC-Namespace3");
            var response = await TestManager.ExecuteActionAsync("Microsoft-GetEventHubPrimaryKey", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task VerifyLogProfileEventHub()
        {
            // Tests the Action to verify that the insights-operational-logs event hub that is created
            // when configuring the Log Profile actually exists
            var dataStore = await TestManager.GetDataStore();
           
            var deployArmResult = await TestManager.ExecuteActionAsync(
                "Microsoft-CreateEventHubNameSpace", dataStore, "Microsoft-ActivityLogTemplate");
            Assert.IsTrue(deployArmResult.IsSuccess);

            var response = await TestManager.ExecuteActionAsync("Microsoft-VerifyLogProfileEventHub", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
