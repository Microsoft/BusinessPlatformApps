using System.IO;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using AzureML;
using AzureML.Contract;
using Microsoft.Azure.Management.MachineLearning.CommitmentPlans;
using Microsoft.Azure.Management.MachineLearning.WebServices;
using Microsoft.Azure.Management.MachineLearning.WebServices.Models;
using Microsoft.Azure.Management.MachineLearning.WebServices.Util;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebService = AzureML.Contract.WebService;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class AzureMLTests
    {
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

            var workspace = workspaces.SingleOrDefault(p => p.Name == "hardcodedwrokspace1");
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
                System.IO.File.WriteAllText(experiment.Description.Replace(".", "").Replace(":", "") + ".json", rawJson);
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
            await DeployAzureMlWorkspaceTest();
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            dataStore.AddToDataStore("WorkspaceName", "test" + TestHarness.RandomCharacters);
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

        [TestMethod]
        public async Task GetWebServiceDefinition()
        {
            // This test will extract all the webservice definitions from AML priceplan
            // Only run when you need it

            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();
            var azureToken = dataStore.GetJson("AzureToken")["access_token"].ToString();
            var subscription = dataStore.GetJson("SelectedSubscription")["SubscriptionId"].ToString();
            var resourceGroup = dataStore.GetValue("SelectedResourceGroup");

            ServiceClientCredentials creds = new TokenCredentials(azureToken);
            AzureMLWebServicesManagementClient client = new AzureMLWebServicesManagementClient(creds);
            AzureMLCommitmentPlansManagementClient commitmentClient = new AzureMLCommitmentPlansManagementClient(creds);
            client.SubscriptionId = subscription;
            commitmentClient.SubscriptionId = subscription;
            var webservices = await client.WebServices.ListAsync();
            foreach (var webserviceName in webservices)
            {
                var webservice = await client.WebServices.GetAsync(resourceGroup, webserviceName.Name);
                var str = ModelsSerializationUtil.GetAzureMLWebServiceDefinitionJsonFromObject(webservice);
                System.IO.File.WriteAllText(webserviceName.Name.Replace(".", "").Replace(":", "") + "WebServiceNew.json", str);
            }
        }
    }
}