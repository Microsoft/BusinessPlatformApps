using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.AzureCustom.IoTContinuousDataExport
{
    [Export(typeof(IAction))]
    public class DeployIoTCDEFunctionCode : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string tokenAzure = request.DataStore.GetJson("AzureToken", "access_token");
            var functionAppName = request.DataStore.GetValue("functionName");

            var zipFileBinary = this.GenerateZipFile(request);

            var client = new AzureHttpClient(tokenAzure);
            var url = $"https://{functionAppName}.scm.azurewebsites.net/api/zipdeploy";

            var result = await client.ExecuteGenericRequestWithHeaderAsync(HttpMethod.Post, url, zipFileBinary);
            if (result.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Success);
            }

            var response = await result.Content.ReadAsStringAsync();
            return new ActionResponse(
                ActionStatus.Failure,
                JsonUtility.GetJObjectFromJsonString(response),
                null,
                DefaultErrorCodes.DefaultErrorCode,
                "Error uploading function code");
        }

        private byte[] GenerateZipFile(ActionRequest request)
        {
            string containerName = request.DataStore.GetValue("SelectedContainer");
            string folder = request.DataStore.GetValue("baseFunctionFolder");

            string deploymentName = request.DataStore.GetValue("DeploymentName") ?? request.DataStore.CurrentRoute;
            string uniqueId = request.DataStore.GetValue("uniqueId");

            var sourceDirectory = new DirectoryInfo(Path.Combine(request.Info.App.AppFilePath, folder));
            var subDirectories = sourceDirectory.GetDirectories();
            var destinationDirectory = new DirectoryInfo(Path.Combine(request.Info.App.AppFilePath, "temp", $"{deploymentName}_{uniqueId}"));

            var hostFile = Path.Combine(sourceDirectory.FullName, "host.json");
            if (File.Exists(hostFile))
            {
                if (!destinationDirectory.Exists)
                {
                    destinationDirectory.Create();
                }

                File.Copy(hostFile, Path.Combine(destinationDirectory.FullName, "host.json"));
            }

            // Copy all files to a destination folder and replace container name
            foreach (var sub in subDirectories)
            {
                var subDestinationFolder = new DirectoryInfo(Path.Combine(destinationDirectory.FullName, sub.Name));
                if (subDestinationFolder.Exists)
                {
                    subDestinationFolder.Delete(true);
                }
                
                subDestinationFolder.Create();

                Directory.GetFiles(sub.FullName, "*.*", SearchOption.AllDirectories)
                    .Select(c => new FileInfo(c))
                    .ToList()
                    .ForEach(c => c.CopyTo(Path.Combine(subDestinationFolder.FullName, c.Name)));

                var functionFile = Path.Combine(subDestinationFolder.FullName, "function.json");
                var functionFileString = File.ReadAllText(functionFile);
                File.WriteAllText(functionFile, functionFileString.Replace("{container}", containerName), System.Text.Encoding.UTF8);
            }

            var destinationFile = Path.Combine(sourceDirectory.FullName, $"{deploymentName}_{uniqueId}_functions.zip");
            if (File.Exists(destinationFile))
            {
                File.Delete(destinationFile);
            }

            ZipFile.CreateFromDirectory(destinationDirectory.FullName, destinationFile);

            var bytes = File.ReadAllBytes(destinationFile);
            destinationDirectory.Delete(true);
            File.Delete(destinationFile);

            return bytes;
        }
    }
}