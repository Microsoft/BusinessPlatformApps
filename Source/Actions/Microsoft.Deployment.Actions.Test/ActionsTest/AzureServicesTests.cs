using System.Threading.Tasks;
using Microsoft.AnalysisServices.Tabular;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class AzureServicesTests
    {
        [TestMethod]
        public async Task DeployAzureAnalysisServices()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("ASServerName", "asserver");
            dataStore.AddToDataStore("ASLocation", "westcentralus");
            dataStore.AddToDataStore("ASSku", "D1");

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureAnalysisServices", dataStore);
            Assert.IsTrue(response.IsSuccess);

            Assert.IsTrue(dataStore.KeyExists("ASServerUrl"));
        }

        [TestMethod]
        public async Task ConnectToAzureAnalysisServices()
         {
            Server server = new Server();
            string connectionString = "Provider=MSOLAP;Data Source=asazure://westcentralus.asazure.windows.net/asserver;User ID=mohaali@microsoft.com;Password=testpass;Persist Security Info=True; Impersonation Level=Impersonate;";
            server.Connect(connectionString);
            Assert.IsTrue(server.Databases.Count > -1);
        }

        [TestMethod]
        public async Task DeployModelToAzureAnalysisServices()
        {
            
        }

        [TestMethod]
        public async Task ProcessModel()
        {
            
        }

    }
}
