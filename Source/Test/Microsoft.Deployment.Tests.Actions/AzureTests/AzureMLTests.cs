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
    public class AzureMLTests
    {
        [TestMethod]
        public async Task ExportExperiments()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore("StorageAccountName", TestManager.ResourceGroup.Replace("-",""));
            dataStore.AddToDataStore("WorkspaceName", "testworkspace");
            //var response = TestManager.ExecuteAction("Microsoft-DeployAzureMLWorkspace", dataStore);
            //Assert.IsTrue(response.IsSuccess);

            //dataStore = new DataStore();
            //dataStore.AddToDataStore("WorkspaceId", "53fe1eac372544239b86cbc44a5c7023");
            //dataStore.AddToDataStore("WorkspaceRegion", "South Central US");
            //dataStore.AddToDataStore("WorkspaceToken", "");
            //response = TestManager.ExecuteAction("Microsoft-ExportAzureMLExperiment", dataStore);
            //Assert.IsTrue(response.IsSuccess);


            dataStore.AddToDataStore("ExperimentJsonPath", "Service/AzureML/Experiments/EntityRecognitionExperiment.json");
            dataStore.AddToDataStore("ExperimentName", "testexperiment");
            var response = await TestManager.ExecuteActionAsync("Microsoft-DeployAzureMLExperiment", dataStore, "Microsoft-NewsTemplate");
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
