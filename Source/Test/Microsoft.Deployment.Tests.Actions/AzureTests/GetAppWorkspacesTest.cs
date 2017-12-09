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
    public class GetAppWorkspacesTest
    {
        [TestMethod]
        public async Task GetGroupsTest()
        {
            var dataStore = await TestManager.GetDataStore(true);
            var response = await TestManager.ExecuteActionAsync("Microsoft-GetAppWorkspaces", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}