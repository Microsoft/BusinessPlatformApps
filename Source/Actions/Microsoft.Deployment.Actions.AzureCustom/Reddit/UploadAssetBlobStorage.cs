using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.WindowsAzure.Storage;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{

    [Export(typeof(IAction))]
    public class UploadAssetBlobStorage : BaseAction
    {
        public const string StorageAccountConnectionStringKey = "StorageAccountConnectionString";
        public const string BlobContainerKey = "BlobContainer";
        public const string AssetFileKey = "AssetFile";
        public const string AccessAssetUriParameterKey = "AccessAssetUriParameter";
        public const string DefaultAccessAssetUriParameter = "AssetAccessUri";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var storageAccountConnectionString = request.DataStore.GetValue(StorageAccountConnectionStringKey);
            var blobContainerName = request.DataStore.GetValue(BlobContainerKey);
            var assetFile = request.DataStore.GetValue(AssetFileKey);
            var accessAssetUriParameter = request.DataStore.GetValue(AccessAssetUriParameterKey);

            if (string.IsNullOrWhiteSpace(accessAssetUriParameter))
            {
                accessAssetUriParameter = DefaultAccessAssetUriParameter;
            }

            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(storageAccountConnectionString))
            {
                errors.Add($"{StorageAccountConnectionStringKey} not defined");
            }

            if (string.IsNullOrWhiteSpace(blobContainerName))
            {
                errors.Add($"{BlobContainerKey} not defined");
            }

            if (string.IsNullOrWhiteSpace(assetFile))
            {
                errors.Add($"{AssetFileKey} not defined");
            }

            if (errors.Count != 0)
            {
                var errorMessage = string.Join("<br/>", errors);
                return new ActionResponse(
                    ActionStatus.Failure,
                    errors,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    errorMessage
                );
            }

            var localPath = $"{request.Info.App.AppFilePath}/{assetFile}";

            if (!File.Exists(localPath))
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    assetFile,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"{AssetFileKey} {assetFile} not found"
                );
            }

            if (!CloudStorageAccount.TryParse(storageAccountConnectionString, out var cloudStorageAccount))
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    null,
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"Azure storage account was not resolvable.  Unable to upload trained model for Azure ML experiment."
                );
            }

            var blobClient = cloudStorageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(blobContainerName);
            await blobContainer.CreateIfNotExistsAsync();
            var cloudBlob = blobContainer.GetBlockBlobReference(FileNameOnly(assetFile));
            using (var fileStream = File.OpenRead(localPath))
            {
                cloudBlob.UploadFromStream(fileStream);
            }

            request.DataStore.AddToDataStore(accessAssetUriParameter, cloudBlob.Uri, DataStoreType.Public);

            return new ActionResponse(ActionStatus.Success);
        }

        private static string FileNameOnly(string path)
        {
            var segments = path.Split('/');
            return segments[segments.Length - 1];
        }
    }
}