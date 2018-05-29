using System;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Tests.Actions.IoTContinuousDataExportTests
{
    [TestClass]
    public class GetAzureStoragesTest
    {
        [TestMethod]
        public async Task GetAzureStorages()
        {
            var dataStore = await TestManager.GetDataStore();

            var response = TestManager.ExecuteAction("Microsoft-GetAzureStorages", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}
