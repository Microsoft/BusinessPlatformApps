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
    }
}