using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.WindowsAzure.Storage.Blob;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PowerApp;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class DeployPowerApp : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string environmentId = request.DataStore.GetValue("PowerAppEnvironment");
            string sqlConnectionId = request.DataStore.GetValue("PowerAppSqlConnectionId");

            try
            {
                if (environmentId != null && sqlConnectionId != null)
                {
                    ActionResponse wrangledFile = await RequestUtility.CallAction(request, "Microsoft-WranglePowerApp");

                    if (wrangledFile.IsSuccess && wrangledFile.Body != null)
                    {
                        string path = wrangledFile.Body.ToString();

                        AzureHttpClient ahc = new AzureHttpClient(azureToken);

                        PowerAppResourceStorage resourceStorage = await ahc.Request<PowerAppResourceStorage>(HttpMethod.Post,
                            string.Format(PowerAppUtility.URL_POWERAPPS_GENERATE_RESOURCE_STORAGE, JsonUtility.GetWebToken(azureToken, "oid")),
                            JsonUtility.Serialize<PowerAppEnvironmentWrapper>(new PowerAppEnvironmentWrapper(environmentId)));

                        if (!string.IsNullOrEmpty(path) && resourceStorage != null && !string.IsNullOrEmpty(resourceStorage.SharedAccessSignature))
                        {
                            string uri = resourceStorage.SharedAccessSignature.Replace("?", "/document.msapp?");

                            CloudBlockBlob blob = new CloudBlockBlob(new Uri(uri));

                            using (WebClient wc = new WebClient())
                            {
                                byte[] file = wc.DownloadData(path);

                                await blob.UploadFromStreamAsync(new MemoryStream(file));

                                PowerAppMetadata metadata = await ahc.Request<PowerAppMetadata>(HttpMethod.Post, PowerAppUtility.URL_POWERAPPS_PUBLISH_APP,
                                    JsonUtility.Serialize<PowerAppPublish>(new PowerAppPublish(uri, $"TwitterTemplate{RandomGenerator.GetDateStamp()}", environmentId, sqlConnectionId)));

                                if (metadata != null)
                                {
                                    if (await ahc.IsSuccess(HttpMethod.Post, string.Format(PowerAppUtility.URL_POWERAPPS_SQL_CONNECTION_UPDATE, sqlConnectionId, environmentId),
                                        JsonUtility.Serialize<PowerAppSqlConnectionUpdate>(new PowerAppSqlConnectionUpdate(metadata.Name))))
                                    {
                                        if (await ahc.IsSuccess(HttpMethod.Post, string.Format(PowerAppUtility.URL_POWERAPPS_RECORD_SCOPES_CONSENT, metadata.Name), JsonUtility.Serialize<PowerAppConsents>(new PowerAppConsents(sqlConnectionId))))
                                        {
                                            request.DataStore.AddToDataStore("PowerAppUri", string.Format(PowerAppUtility.URL_POWERAPPS_WEB, metadata.Name));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                PowerAppUtility.SkipPowerApp(request.DataStore);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}