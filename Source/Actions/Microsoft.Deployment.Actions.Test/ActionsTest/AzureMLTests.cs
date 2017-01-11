using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using AzureML.Contract;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class AzureMLTests
    {
        [Ignore] //missing file
        [TestMethod]
        public async Task DeployAzureMlWorkspaceTest()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("WorkspaceName", "test" + TestHarness.RandomCharacters);
            dataStore.AddToDataStore("StorageAccountName", "testazuremlstorage" + TestHarness.RandomCharacters);
            dataStore.AddToDataStore("DeploymentName", "MLWorkspaceDeployment");

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureMLWorkspace", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public async Task GetExperimentsTest()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            ManagementSDK sdk = new ManagementSDK();
            var workspaces = sdk.GetWorkspacesFromRdfe(dataStore.GetJson("AzureToken")["access_token"].ToString(),
                dataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString());

            var workspaceSettings = new WorkspaceSetting()
            {
                AuthorizationToken = workspaces[0].AuthorizationToken.PrimaryToken,
                Location = workspaces[0].Region,
                WorkspaceId = workspaces[0].Id
            };
            var experiments = sdk.GetExperiments(workspaceSettings);
            string rawJson = string.Empty;
            Experiment exp = sdk.GetExperimentById(workspaceSettings, experiments[0].ExperimentId, out rawJson);
        }

        [TestMethod]
        public async Task ExportExperiment()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            ManagementSDK sdk = new ManagementSDK();
            var workspaces = sdk.GetWorkspacesFromRdfe(dataStore.GetJson("AzureToken")["access_token"].ToString(),
                dataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString());

           var workspace =  workspaces.SingleOrDefault(p => p.Name == "testdlkbt");
            var workspaceSettings = new WorkspaceSetting()
            {
                AuthorizationToken = workspace.AuthorizationToken.PrimaryToken,
                Location = workspace.Region,
                WorkspaceId = workspace.Id
            };

            var experiments = sdk.GetExperiments(workspaceSettings);
            foreach (var experiment in experiments)
            {
                string rawJson = string.Empty;
                Experiment exp = sdk.GetExperimentById(workspaceSettings, experiment.ExperimentId, out rawJson);
                System.IO.File.WriteAllText(experiment.Description.Replace(".","").Replace(":","") + ".json", rawJson);
                
            }
            
        }

        [TestMethod]
        public async Task GetWorkspacesByNameTest()
        {
            await DeployAzureMlWorkspaceTest();
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("WorkspaceName", "test" + TestHarness.RandomCharacters);
            var response = TestHarness.ExecuteAction("Microsoft-GetAzureMLWorkspaceByName", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public async Task DeployAzureMlExperiment()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("WorkspaceName", "testdlkbt");
            dataStore.AddToDataStore("ExperimentJsonPath", "Service/AzureML/Experiments/Topics.json");
            dataStore.AddToDataStore("ExperimentName", "TopicsDeployed");
            dataStore.AddToDataStore("SqlConnectionString", TestHarness.GetSqlPagePayload("testruns"));
            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureMLExperiment", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            response = TestHarness.ExecuteAction("Microsoft-InsertDatabaseCredentialsIntoExperiment", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            response = TestHarness.ExecuteAction("Microsoft-DeployAzureMLWebService", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            response = TestHarness.ExecuteAction("Microsoft-WaitForAzureMLWebServiceCreation", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }
    }
}