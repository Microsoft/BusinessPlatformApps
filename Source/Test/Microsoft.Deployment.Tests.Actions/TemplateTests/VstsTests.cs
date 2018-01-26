using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Tests.Actions.TemplateTests
{
    [TestClass]
    public class VstsTests
    {
        [TestMethod]
        public async Task ValidateSQL()
        {
            var dataStore =  await TestManager.GetDataStore();
            dataStore.AddToDataStore("FacebookClientId", "422676881457852");
            dataStore.AddToDataStore("FacebookClientSecret", "bf5fca097936ece936290031623b577b");
            dataStore.AddToDataStore("SqlConnectionString", "Server=tcp:testmodb.database.windows.net,1433;Initial Catalog=apiManagementDB;Persist Security Info=False;User ID=pbiadmin;Password=Corp123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            dataStore.AddToDataStore("Schema", "fb");
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Database");

            ActionResponse response = null;

            response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "BlueMargin-VSTS");
            Assert.IsTrue(response.IsSuccess);

            response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "BlueMargin-VSTS");
            Assert.IsTrue(response.IsSuccess);
        }
    }
}

