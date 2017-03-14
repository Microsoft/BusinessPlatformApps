using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using System.Threading;
using System;
using System.Collections.Generic;

namespace Microsoft.Deployment.Actions.Common.PBI
{
    [Export(typeof(IAction))]
    public class WranglePBISSAS : BaseAction
    {
        private const int RETRY_ATTEMPTS = 10;

        private FileStream AVAwareOpen(string path, FileMode mode, FileAccess access, FileShare share)
        {
            FileStream result = null;
            List<Exception> exceptionList = new List<Exception>();

            for (int i = 0; i < RETRY_ATTEMPTS; i++)
            {
                try
                {
                    result = new FileStream(path, mode, access, share);
                    break;
                }
                catch (Exception e)
                {
                    exceptionList.Add(e);
                    Thread.Sleep(250);
                    continue;
                }
            }

            if (result == null)
                throw new AggregateException($"Could not open file: {path}", exceptionList);

            return result;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string asDatabase = request.DataStore.GetValue("ASDatabase");
            string asServer = request.DataStore.GetValue("ASServerUrl");

            string[] originalFiles = request.DataStore.GetValue("FileNameSSAS").Split('|');
            string[] tempFolders = new string[originalFiles.Length];

            for (int i = 0; i < originalFiles.Length; i++)
            {
                string templateFullPath = request.Info.App.AppFilePath + $"/service/PowerBI/{originalFiles[i]}";
                tempFolders[i] = Path.GetRandomFileName();
                Directory.CreateDirectory(request.Info.App.AppFilePath + $"/Temp/{tempFolders[i]}");

                using (PBIXUtils wrangler = new PBIXUtils(templateFullPath, request.Info.App.AppFilePath + $"/Temp/{tempFolders[i]}/{originalFiles[i]}"))
                {
                    wrangler.ReplaceSSASConnectionString(asServer, asDatabase, "model");
                }
            }

            string serverPath = string.Empty;
            if (originalFiles.Length == 1)
            {
                serverPath = request.Info.ServiceRootUrl + request.Info.ServiceRelativePath + request.Info.App.AppRelativeFilePath + $"/Temp/{tempFolders[0]}/{originalFiles[0]}";
            }
            else
            {
                string randomZipFolder = Path.GetRandomFileName();
                DirectoryInfo d =  Directory.CreateDirectory(Path.Combine(request.Info.App.AppFilePath, "Temp", randomZipFolder));
                serverPath = Path.Combine(d.FullName, "SolutionTemplate.zip");
                using (FileStream zipFile = AVAwareOpen(serverPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (ZipArchive z = new ZipArchive(zipFile, ZipArchiveMode.Update))
                    {
                        for (int i = 0; i < originalFiles.Length; i++)
                        {
                            ZipArchiveEntry entry = z.CreateEntry(originalFiles[i], CompressionLevel.Optimal);
                            using (Stream w = entry.Open())
                            {
                                string fileToZip = Path.Combine(request.Info.App.AppFilePath, "Temp", tempFolders[i], originalFiles[i]);
                                using (FileStream source = AVAwareOpen(fileToZip, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    source.CopyTo(w);
                                    w.Flush();
                                }
                            }
                        }
                    }
                }

                // reconstruct a web server path
                serverPath = request.Info.ServiceRootUrl + request.Info.ServiceRelativePath + request.Info.App.AppRelativeFilePath + $"/Temp/{randomZipFolder}/SolutionTemplate.zip";;
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(serverPath));
        }
    }
}