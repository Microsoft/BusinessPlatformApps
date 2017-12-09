using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureSql
{
    [Export(typeof(IAction))]
    class GetTextBlob : BaseAction
    {

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            CloudBlobClient blobClient = new CloudBlobClient(new Uri(request.DataStore.GetValue("storageBaseUrl")));
            CloudBlob blob = (CloudBlob)blobClient.GetBlobReferenceFromServer(new Uri(request.DataStore.GetValue("blobUrl")));
            string contentName = request.DataStore.GetValue("blobContentName");

            string content = string.Empty;
            using (StreamReader sr = new StreamReader(blob.OpenRead()))
            {
                content = sr.ReadLine();
            }

            request.DataStore.AddToDataStore(contentName, content, DataStoreType.Public);

            return new ActionResponse(ActionStatus.Success);
        }
    }
}
