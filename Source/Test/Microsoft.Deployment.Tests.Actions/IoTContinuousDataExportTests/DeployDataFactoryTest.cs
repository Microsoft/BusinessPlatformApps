using System;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Tests.Actions.IoTContinuousDataExportTests
{
    [TestClass]
    public class DeployDataFactoryTest
    {
        [TestMethod]
        public async Task CreateDataFactory()
        {
            var dataStore = await TestManager.GetDataStore();

            // Deploy Data Factory
            dataStore.AddToDataStore("DeploymentName", "DataFactoryDeploymentTest");

            string valueUnique = GetUniqueString(dataStore.GetJson("SelectedSubscription", "SubscriptionId"));
            dataStore.AddToDataStore("uniqueId", valueUnique);

            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/datafactory.json");
            dataStore.AddToDataStore("SqlConnectionString", "test connection string");

            //dataStore.AddToDataStore("dataFactoryName", $"iot-cde-dataf-{valueUnique}");
            dataStore.AddToDataStore("dataFactoryName", Guid.NewGuid());

            var response = TestManager.ExecuteAction("Microsoft-DeployIoTCDEDataFactory", dataStore, "Microsoft-IoTContinuousDataExportTemplate");
            Assert.IsTrue(response.IsSuccess);
        }

        private static string GetUniqueString(string id, int length = 10)
        {
            string result = "";
            var buffer = System.Text.Encoding.UTF8.GetBytes(id);
            var hashArray = new System.Security.Cryptography.SHA512Managed().ComputeHash(buffer);
            for (int i = 1; i <= length; i++)
            {
                var b = hashArray[i];
                var c = Convert.ToChar((b % 26) + (byte)'a');
                result = result + c;
            }

            return result;
        }
    }
}
