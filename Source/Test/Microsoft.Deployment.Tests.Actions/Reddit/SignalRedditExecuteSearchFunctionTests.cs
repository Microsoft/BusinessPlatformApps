using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.AzureCustom.Reddit;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;

namespace Microsoft.Deployment.Tests.Actions.Reddit
{
    [TestClass]
    public class SignalRedditExecuteSearchFunctionTests
    {
      
        private const string storageQueueName = "line";

        private readonly string workingStorageAccountConnectionString = Credential.Instance.StorageAccount.StorageAccountConnectionString;
        private readonly string brokenStorageAccountConnectionString = Credential.Instance.StorageAccount.StorageAccountConnectionString.Replace("Account", "ACC");

        [TestMethod]
        public async Task MissingArguments()
        {
            var dataStore = await TestManager.GetDataStore();

            var response = await TestManager.ExecuteActionAsync("Microsoft-SignalExecuteRedditSearchFunction", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure);
            var expected = new List<string>()
            {
                $"{SignalExecuteRedditSearchFunction.StorageAccountConnectionStringKey} not defined",
                $"{SignalExecuteRedditSearchFunction.StorageQueueNameKey} not defined"
            };
            Assert.IsInstanceOfType(response.Body, typeof(List<string>));
            CollectionAssert.AreEqual(expected, (List<string>)response.Body);
        }

        // should be enabled once a valid StorageAccount.StorageAccountConnectionString is put in the credential.json
        [Ignore]
        [TestMethod]
        public async Task BadConnectionString()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(SignalExecuteRedditSearchFunction.StorageAccountConnectionStringKey, brokenStorageAccountConnectionString, DataStoreType.Public);
            dataStore.AddToDataStore(SignalExecuteRedditSearchFunction.StorageQueueNameKey, storageQueueName, DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-SignalExecuteRedditSearchFunction", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure);
            Assert.AreEqual("Azure storage account was not resolvable.  This is required to start function processing", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        // should be enabled once a valid StorageAccount.StorageAccountConnectionString is put in the credential.json
        [Ignore]
        [TestMethod]
        public async Task TestAddToQueue()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(SignalExecuteRedditSearchFunction.StorageAccountConnectionStringKey, workingStorageAccountConnectionString, DataStoreType.Public);
            dataStore.AddToDataStore(SignalExecuteRedditSearchFunction.StorageQueueNameKey, storageQueueName, DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-SignalExecuteRedditSearchFunction", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestCleanup]
        public void CleanQueue()
        {
            if (!CloudStorageAccount.TryParse(workingStorageAccountConnectionString, out var cloudStorageAccount))
            {
                Debug.WriteLine($"Unable to connect to {workingStorageAccountConnectionString} to remove queue {storageQueueName}");
            }
            else
            {
                var queueClient = cloudStorageAccount.CreateCloudQueueClient();
                var cloudQueue = queueClient.GetQueueReference(storageQueueName);
                cloudQueue.DeleteIfExists();
            }
        }
        
    }
}