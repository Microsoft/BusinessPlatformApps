using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.Facebook
{
    [TestClass]
    public class AcademicTests
    {
        [TestMethod]
        public async Task DeployAcademicTemplate()
        {
            var dataStore =  await TestManager.GetDataStore();
            dataStore.AddToDataStore("SqlConnectionString", "Server=tcp:modb1.database.windows.net,1433;Initial Catalog=fb;Persist Security Info=False;User ID=pbiadmin;Password=Corp123!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
            dataStore.AddToDataStore("Schema", "ak");
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Database");
  
            ActionResponse response = null;

            //response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-AcademicTemplate");
            //Assert.IsTrue(response.IsSuccess);

            //// Testing to see if the tear down works
            //response = await TestManager.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-AcademicTemplate");
            //Assert.IsTrue(response.IsSuccess);

            // Academic
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest1");
            dataStore.AddToDataStore("CognitiveServiceName", "AcademicCognitiveService");
            dataStore.AddToDataStore("CognitiveServiceType", "Academic");
            dataStore.AddToDataStore("CognitiveSkuName", "S0");

            response = TestManager.ExecuteAction("Microsoft-DeployCognitiveService", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestManager.ExecuteAction("Microsoft-GetCognitiveKey", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //response = await TestManager.ExecuteActionAsync("Microsoft-WaitForCognitiveService", dataStore, "Microsoft-AcademicTemplate");
            //Assert.IsTrue(response.IsSuccess);
        }
    }
}