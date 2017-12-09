using System;
using System.IO;
using System.IO.Compression;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Win32.TaskScheduler;
using Microsoft.WindowsAzure.Storage.Blob;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

using Task = Microsoft.Win32.TaskScheduler.Task;

namespace Microsoft.Deployment.Actions.OnPremise.TaskScheduler
{
    [Export(typeof(IAction))]
    public class GetTaskStatus : BaseAction
    {
        static String userLogDir = "C:\\ProgramData\\Business Platform Solution Templates\\Microsoft-SCCMTemplate\\Logs";
        static String exportLogDir = "C:\\ProgramData\\Business Platform Solution Templates\\Microsoft-SCCMTemplate\\Logs\\Export";

        public Task GetScheduledTask(string taskName)
        {
            using (TaskService ts = new TaskService())
            {
                TaskCollection tasks = ts.RootFolder.GetTasks(new Regex(taskName));
                if (tasks != null && tasks.Count > 0)
                    return tasks[0]; // We expect only one task to match
                else
                    return null;
            }
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            // The status could change if we're patient
            System.Threading.Thread.Sleep(250);

            Task task = GetScheduledTask(request.DataStore.GetValue("TaskName"));
            if (task == null)
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "SccmTaskNotFound");

            // Let it run
            if (task.State == TaskState.Queued || task.State == TaskState.Running || task.State == TaskState.Unknown)
                return new ActionResponse(ActionStatus.InProgress);

            switch (task.LastTaskResult)
            {
                case 0:
                    return new ActionResponse(ActionStatus.Success);
                case 0x00041303: // SCHED_S_TASK_HAS_NOT_RUN: The task has not yet run.
                case 0x00041325: // SCHED_S_TASK_QUEUED: The Task Scheduler service has asked the task to run
                    return new ActionResponse(ActionStatus.InProgress); 
                default:
                    // there was an error since we haven't exited above
                    uploadLogs(request);
                    if (NTHelper.IsCredentialGuardEnabled())
                        return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "CredentialGuardEnabled");

                    // azurebcp exits with 0, 1, 2, the powershell script might return 1 - anything else must be a Windows error
                    Exception e = (uint)task.LastTaskResult > 2 ?
                                                    new Exception($"Scheduled task exited with code {task.LastTaskResult}", new System.ComponentModel.Win32Exception(task.LastTaskResult)) :
                                                    new Exception($"Scheduled task exited with code {task.LastTaskResult}");

                    ActionResponse response = new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), e, "TaskSchedulerRunFailed");
                    response.ExceptionDetail.LogLocation = FileUtility.GetLocalTemplatePath(request.Info.AppName);
                    return response;
            }
        }


        private void uploadLogs(ActionRequest request)
        {
            zipLogs();

            // Full URL: https://pbiststorage.blob.core.windows.net/?sv=2016-05-31&ss=b&srt=o&sp=wa&se=2018-06-07T13:00:00Z&st=2017-06-07T13:00:00Z&spr=https&sig=v%2BPr50XV5WDHxvE1YiNxqtSyB67li0C6EFgQnI%2B9Wc0%3D
            string token = "?sv=2016-05-31&ss=b&srt=o&sp=wa&se=2018-06-07T13:00:00Z&st=2017-06-07T13:00:00Z&spr=https&sig=v%2BPr50XV5WDHxvE1YiNxqtSyB67li0C6EFgQnI%2B9Wc0%3D";
            string url = "https://pbiststorage.blob.core.windows.net/sccmlogs/";

            string username = request.Info.UserGenId;

            try
            {
                if (Directory.Exists(exportLogDir))
                {
                    var logFiles = Directory.EnumerateFiles(exportLogDir);
                    foreach (string filePath in logFiles)
                    {
                        string[] pathBits = filePath.Split('\\');
                        string filename = pathBits[pathBits.Length - 1];
                        string cloudFilename = filename.Replace("_", "." + username + "." );
                        string sasUri = url + cloudFilename + token;
                        FileStream writeStream = new FileStream(filePath, FileMode.Open);
                        writeStream.Position = 0;
                        CloudBlockBlob blob = new CloudBlockBlob(new Uri(sasUri));
                        blob.UploadFromStream(writeStream);
                    }
                }
            }
            catch (Exception e)
            {
                request.Logger.LogEvent("Upload azurebcp logs failed");
                request.Logger.LogException(e);
            }
        }

        private void zipLogs()
        {
            if (Directory.Exists(userLogDir))
            {
                if (Directory.Exists(exportLogDir))
                {
                    Directory.Delete(exportLogDir, true);
                }
                Directory.CreateDirectory(exportLogDir);

                //copy the logs first.  (If you try to do a read on the original log file, you get an error saying another process is using it.)
                var logFiles = Directory.EnumerateFiles(userLogDir);
                foreach (string filePath in logFiles)
                {
                    string[] pathBits = filePath.Split('\\');
                    string fileName = pathBits[pathBits.Length - 1];
                    int year = int.Parse(fileName.Substring(9, 4));
                    int month = int.Parse(fileName.Substring(13, 2));
                    int day = int.Parse(fileName.Substring(15, 2));
                    DateTime fileDate = new DateTime(year, month, day);
                    DateTime wantNewer = DateTime.Now.AddDays(-7);
                    if (fileDate > wantNewer)
                    {
                        string newFilePath = Path.Combine(exportLogDir, fileName);
                        File.Copy(filePath, newFilePath, true);
                    }
                }

                foreach (FileInfo fileToCompress in new DirectoryInfo(exportLogDir).GetFiles())
                {
                    using (FileStream originalFileStream = fileToCompress.OpenRead())
                    {
                        if ((File.GetAttributes(fileToCompress.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fileToCompress.Extension != ".gz")
                        {
                            string newFileName = Path.Combine(exportLogDir, fileToCompress.Name) + ".gz";
                            using (FileStream compressedFileStream = File.Create(newFileName))
                            {
                                using (GZipStream compressionStream = new GZipStream(compressedFileStream, CompressionMode.Compress))
                                {
                                    originalFileStream.CopyTo(compressionStream);
                                }
                            }
                        }
                    }
                    fileToCompress.Delete();
                }
            }
        }

    }
}