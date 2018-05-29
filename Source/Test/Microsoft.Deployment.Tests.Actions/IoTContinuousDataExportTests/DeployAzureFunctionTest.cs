using System;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Tests.Actions.IoTContinuousDataExportTests
{
    [TestClass]
    public class DeployAzureFunctionTest
    {
        [TestMethod]
        public async Task CreateAzureFunction()
        {
            var dataStore = await TestManager.GetDataStore();

            // Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");

            string valueUnique = GetUniqueString(dataStore.GetJson("SelectedSubscription", "SubscriptionId"));
            dataStore.AddToDataStore("uniqueId", valueUnique);
            var jObject = new JObject
            {
                ["StorageAccountConnectionString"] = "test"
            };

            dataStore.AddToDataStore("AzureArmFile", "Service/ARM/functions.json");
            dataStore.AddToDataStore("SqlConnectionString", "test connection string");
            dataStore.AddToDataStore("SelectedStorageAccount", jObject);
            dataStore.AddToDataStore("functionName", $"iot-cde-func-{valueUnique}");
            dataStore.AddToDataStore("functionStorageAccountName", $"func{valueUnique}");

            var response = TestManager.ExecuteAction("Microsoft-DeployIoTCDEAzureFunction", dataStore, "Microsoft-IoTContinuousDataExportTemplate");
            Assert.IsTrue(response.IsSuccess);
        }

        [TestMethod]
        public async Task DeployFunctionCode()
        {
            var dataStore = await TestManager.GetDataStore();

            // Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");

            string valueUnique = GetUniqueString(dataStore.GetJson("SelectedSubscription", "SubscriptionId"));
            dataStore.AddToDataStore("uniqueId", valueUnique);

            dataStore.AddToDataStore("functionName", $"iot-cde-func-{valueUnique}");
            dataStore.AddToDataStore("SelectedContainer", "iothub");

            //var response = TestManager.ExecuteAction("Microsoft-DeployIoTCDEAzureFunction", dataStore, "Microsoft-IoTContinuousDataExportTemplate");
            //Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("baseFunctionFolder", "service/data");
            var response = TestManager.ExecuteAction("Microsoft-DeployIoTCDEFunctionCode", dataStore, "Microsoft-IoTContinuousDataExportTemplate");
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
