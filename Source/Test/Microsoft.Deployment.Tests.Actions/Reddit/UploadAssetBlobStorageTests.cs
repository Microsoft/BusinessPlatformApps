using System;
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
    public class UploadAssetBlobStorageTest
    {
        private const string ImageFileName = "reddit-architecture.png";
        private readonly string testImageLocation = $"Web/Images/{ImageFileName}";
        private readonly string blobContainerName = $"test{DateTime.Now.Ticks}";

        private readonly string workingStorageAccountConnectionString = Credential.Instance.StorageAccount.StorageAccountConnectionString;
        private readonly string brokenStorageAccountConnectionString = Credential.Instance.StorageAccount.StorageAccountConnectionString.Replace("Account", "ACC");
        
        [TestMethod]
        public async Task MissingArguments()
        {
            var dataStore = await TestManager.GetDataStore();

            var response = await TestManager.ExecuteActionAsync("Microsoft-UploadAssetBlobStorage", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure);
            var expected = new List<string>()
            {
                $"{UploadAssetBlobStorage.StorageAccountConnectionStringKey} not defined",
                $"{UploadAssetBlobStorage.BlobContainerKey} not defined",
                $"{UploadAssetBlobStorage.AssetFileKey} not defined"
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
            dataStore.AddToDataStore(UploadAssetBlobStorage.BlobContainerKey, blobContainerName, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.AssetFileKey, testImageLocation, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.StorageAccountConnectionStringKey, brokenStorageAccountConnectionString, DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-UploadAssetBlobStorage", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure);
            Assert.AreEqual("Azure storage account was not resolvable.  Unable to upload trained model for Azure ML experiment.", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        // should be enabled once a valid StorageAccount.StorageAccountConnectionString is put in the credential.json
        [Ignore]
        [TestMethod]
        public async Task LocalFileNotFound()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(UploadAssetBlobStorage.BlobContainerKey, blobContainerName, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.AssetFileKey, "file/not/found.jpg", DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.StorageAccountConnectionStringKey, workingStorageAccountConnectionString, DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-UploadAssetBlobStorage", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Failure);
            Assert.IsInstanceOfType(response.Body, typeof(string));
            Assert.AreEqual("file/not/found.jpg", response.Body.ToString());
            Assert.AreEqual($"{UploadAssetBlobStorage.AssetFileKey} file/not/found.jpg not found", response.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        // should be enabled once a valid StorageAccount.StorageAccountConnectionString is put in the credential.json
        [Ignore]
        [TestMethod]
        public async Task TestUploadAssetToAzure()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(UploadAssetBlobStorage.BlobContainerKey, blobContainerName, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.AssetFileKey, testImageLocation, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.StorageAccountConnectionStringKey, workingStorageAccountConnectionString, DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-UploadAssetBlobStorage", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);
            var uriString = dataStore.GetValue(UploadAssetBlobStorage.DefaultAccessAssetUriParameter);
            Assert.IsTrue(uriString.Contains($"{blobContainerName}/{ImageFileName}"));
        }

        // should be enabled once a valid StorageAccount.StorageAccountConnectionString is put in the credential.json
        [Ignore]
        [TestMethod]
        public async Task TestUploadAssetToAzure_CustomUriParameter()
        {
            var dataStore = await TestManager.GetDataStore();
            dataStore.AddToDataStore(UploadAssetBlobStorage.BlobContainerKey, blobContainerName, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.AssetFileKey, testImageLocation, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.StorageAccountConnectionStringKey, workingStorageAccountConnectionString, DataStoreType.Public);
            dataStore.AddToDataStore(UploadAssetBlobStorage.AccessAssetUriParameterKey, "custom", DataStoreType.Public);

            var response = await TestManager.ExecuteActionAsync("Microsoft-UploadAssetBlobStorage", dataStore, "Microsoft-RedditTemplate");
            Assert.IsTrue(response.Status == ActionStatus.Success);
            var uriString = dataStore.GetValue("custom");
            Assert.IsTrue(uriString.Contains($"{blobContainerName}/{ImageFileName}"));
        }

        [TestCleanup]
        public void CleanStorage()
        {
            if (!CloudStorageAccount.TryParse(workingStorageAccountConnectionString, out var cloudStorageAccount))
            {
                Debug.WriteLine($"Unable to connect to {workingStorageAccountConnectionString} to remove test container {blobContainerName}");
            }
            else
            {
                var blobClient = cloudStorageAccount.CreateCloudBlobClient();
                var blobContainer = blobClient.GetContainerReference(blobContainerName);
                blobContainer.DeleteIfExists();
            }
        }
        
    }
}