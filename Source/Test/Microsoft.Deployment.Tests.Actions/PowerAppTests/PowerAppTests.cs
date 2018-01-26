using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.PowerApp
{
    [TestClass]
    public class PowerAppTests
    {
        [TestMethod]
        public async Task ValidateConfigurePowerAppTables()
        {
            DataStore ds = new DataStore();

            ds.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("yashti"));
            ds.AddToDataStore("SqlScriptsFolder", "Service/Database/LogicApps");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-DeploySQLScripts", ds, "Microsoft-TwitterTemplate"));

            //ds.AddToDataStore("SearchQuery", "cricket rights OR english cricket OR \"Sky AND cricket\"");
            //ds.AddToDataStore("SearchQuery", "FA and women's performance");
            ds.AddToDataStore("SearchQuery", "LCW OR “lc Waikiki”");

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-ConfigurePowerAppTables", ds));
        }

        [TestMethod]
        public async Task ValidatePowerAppDeployment()
        {
            DataStore ds =  await TestManager.GetDataStore();

            ds.AddToDataStore("PowerAppFileName", "TwitterTemplate.msapp");
            ds.AddToDataStore("SqlConnectionString", SqlCreds.GetSqlPagePayload("yashti"));

            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-GetPowerAppEnvironment", ds));
            Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-CreatePowerAppSqlConnection", ds));
            //Assert.IsTrue(await TestManager.IsSuccessAsync("Microsoft-DeployPowerApp", ds, "Microsoft-TwitterTemplate"));
        }
    }
}