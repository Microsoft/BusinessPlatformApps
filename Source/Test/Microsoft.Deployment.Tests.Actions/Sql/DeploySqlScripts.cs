using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.Sql
{
    [TestClass]
    public class DeploySqlScripts
    {

        [TestMethod]
        public async Task DeploySqlScriptsTest()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore();

            // Deploy Twitter Database Scripts
            dataStore.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("ssas"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/LogicApps");

            var response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-TwitterTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);

            // Deploy AS Model based of the following pramaters
            dataStore = await TestManager.GetDataStore();

            // Deploy Twitter Database Scripts
            dataStore.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("ssas"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database");

            response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public async Task DeploySCCM()
        {
            // Deploy AS Model based of the following pramaters
            var dataStore = await TestManager.GetDataStore();

            // Deploy Twitter Database Scripts
            dataStore.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("modb1"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/noetl");

            var response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-SCCMTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }
    }
}
