using System.IO;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;


    [TestClass]
    public class OnPremiseTests
    {
        public static string rootAppPath = FileUtility.GetLocalTemplatePath(TestHarness.TemplateName);
        public static string targetPath = rootAppPath + "\\" + Path.GetRandomFileName();

        [TestMethod]
        [Ignore]
        public void CreateTaskWithCorrectCredentialsSuccess()
        {
            var dataStore = new DataStore();

            dataStore.AddToDataStore("ImpersonationUsername", "TestUser");
            dataStore.AddToDataStore("ImpersonationPassword", "TestP@ssw0rd");
            dataStore.AddToDataStore("ImpersonationDomain", "\\");

            dataStore.AddToDataStore("TaskDescription", "Test Task Description");
            dataStore.AddToDataStore("TaskFile", "test-file.ps1");
            dataStore.AddToDataStore("TaskLocation", "Business Platform Solution Templates\\Test\\");
            dataStore.AddToDataStore("TaskName", "Test Task Name");
            dataStore.AddToDataStore("TaskParameters", "-ExecutionPolicy Bypass");
            dataStore.AddToDataStore("TaskProgram", "powershell");
            dataStore.AddToDataStore("TaskStartTime", "2:00");

            var result = TestHarness.ExecuteAction("Microsoft-CreateTask", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        [Ignore]
        public void RemoveTaskSuccess()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("TaskName", "Test Task Name");

            var result = TestHarness.ExecuteAction("Microsoft-RemoveTask", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [Ignore]
        public void InstallSCCMSuccess()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("TargetPath", targetPath);
            var result = TestHarness.ExecuteAction("Microsoft-InstallSCCM", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [Ignore]
        [TestMethod]
        public void RemoveFilesSuccess()
        {
            this.InstallSCCMSuccess();

            var dataStore = new DataStore();
            dataStore.AddToDataStore("TargetPath", "Apps\\Microsoft\\Released\\Microsoft-SCCMTemplate\\Temp");
            var result = TestHarness.ExecuteAction("Microsoft-RemoveFiles", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void CreateCredentialEntrySuccess()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("CredentialTarget", "pbi_sccm", DataStoreType.Public);
            dataStore.AddToDataStore("CredentialUsername", "testUsername", DataStoreType.Public);
            dataStore.AddToDataStore("CredentialPassword", "testPassword", DataStoreType.Private);
            var result = TestHarness.ExecuteAction("Microsoft-CredentialManagerWrite", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public void DeleteCredentialEntrySuccess()
        {
            var dataStore = new DataStore();
            dataStore.AddToDataStore("CredentialTarget", "pbi_sccm", DataStoreType.Public);
            dataStore.AddToDataStore("CredentialUsername", "testUsername", DataStoreType.Public);
            var result = TestHarness.ExecuteAction("Microsoft-CredentialManagerDelete", dataStore);
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task GetCurrentUserDoesNotFailTest()
        {
            var response = await TestHarness.ExecuteActionAsync("Microsoft-GetCurrentUserAndDomain", new DataStore());
            Assert.IsTrue(response.IsSuccess);
        }
    }
}