using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.APIManagement
{
    [TestClass]
    public class APIManagementTests
    {
        [TestMethod]
        public async Task ValidateAPIManagementDBScripts()
        {
            DataStore ds = new DataStore();

            ds.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("yashti"));
            ds.AddToDataStore("SqlScriptsFolder", "Database");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-DeploySQLScripts", ds, "Microsoft-APIManagementTemplate"));
        }
    }
}