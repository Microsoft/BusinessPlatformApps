using System;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PBI;

namespace Microsoft.Deployment.Actions.Common.PBI
{
    [Export(typeof(IAction))]
    public class PublishPBIReportCDSA : BaseAction
    {
        private const int MAXIMUM_IMPORT_STATUS_ATTEMPTS = 92;
        private const int WAIT_IMPORT_STATUS = 5;

        private const string PBI_IMPORT_STATUS_URI = "v1.0/myorg/{0}imports/{1}";
        private const string PBI_IMPORT_URI = "v1.0/myorg/{0}imports/?datasetDisplayName={1}&nameConflict=Abort";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            AzureHttpClient ahc = new AzureHttpClient(request.DataStore.GetJson("PBIToken", "access_token"));
            string pbiClusterUri = request.DataStore.GetValue("PBIClusterUri");
            string pbiWorkspaceId = request.DataStore.GetValue("PBIWorkspaceId");
            string pbixLocation = request.DataStore.GetValue("PBIXLocation");

            pbiWorkspaceId = string.IsNullOrEmpty(pbiWorkspaceId) ? string.Empty : "groups/" + pbiWorkspaceId + "/";

            byte[] file = null;
            using (WebClient wc = new WebClient())
            {
                file = wc.DownloadData(pbixLocation);
            }

            string filename = request.Info.AppName + RandomGenerator.GetDateStamp() + ".pbix";
            PBIImport pbiImport = JsonUtility.Deserialize<PBIImport>(await ahc.Request(pbiClusterUri + string.Format(PBI_IMPORT_URI, pbiWorkspaceId, filename), file, filename));

            PBIImportStatus pbiImportStatus = null;
            int attempts = 0;
            bool isImportInProgress = true;
            while (isImportInProgress && attempts < MAXIMUM_IMPORT_STATUS_ATTEMPTS)
            {
                pbiImportStatus = await ahc.Request<PBIImportStatus>(HttpMethod.Get, pbiClusterUri + string.Format(PBI_IMPORT_STATUS_URI, pbiWorkspaceId, pbiImport.Id));
                switch (pbiImportStatus.ImportState)
                {
                    case "Publishing":
                        Thread.Sleep(new TimeSpan(0, 0, WAIT_IMPORT_STATUS));
                        break;
                    case "Succeeded":
                        isImportInProgress = false;
                        break;
                    default:
                        isImportInProgress = false;
                        break;
                }
                attempts++;
            }

            string reportUrl = pbiImportStatus == null || pbiImportStatus.Reports == null || pbiImportStatus.Reports.Count == 0 ? string.Empty : pbiImportStatus.Reports[0].WebUrl;

            string pbixDatasetId = pbiImportStatus == null || pbiImportStatus.Datasets == null || pbiImportStatus.Datasets.Count == 0 ? string.Empty : pbiImportStatus.Datasets[0].Id;

            request.DataStore.AddToDataStore("PBIXDatasetId", pbixDatasetId, DataStoreType.Public);

            return new ActionResponse(ActionStatus.Success, reportUrl);
        }
    }
}