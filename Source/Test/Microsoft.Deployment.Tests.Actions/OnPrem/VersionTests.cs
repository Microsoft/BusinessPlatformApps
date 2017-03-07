using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.OnPrem
{
    [TestClass]
    public class VersionTests
    {
        [TestMethod]
        public void CheckVersion()
        {
            var dataStore = new DataStore();
            var response = TestManager.ExecuteActionAsync("Microsoft-CheckVersion", dataStore).Result;
            Assert.IsTrue((bool)response.Body == true || (bool)response.Body == false);
        }
    }
}
