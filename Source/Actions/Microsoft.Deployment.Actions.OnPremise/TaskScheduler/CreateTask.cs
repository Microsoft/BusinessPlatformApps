using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.Win32.TaskScheduler;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

using Task = Microsoft.Win32.TaskScheduler.Task;

namespace Microsoft.Deployment.Actions.OnPremise.TaskScheduler
{
    [Export(typeof(IAction))]
    public class CreateTask : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var taskDescription = request.DataStore.GetValue("TaskDescription");
            var taskFile = request.DataStore.GetValue("TaskFile");
            var taskName = request.DataStore.GetValue("TaskName");
            var taskParameters = request.DataStore.GetValue("TaskParameters");
            var taskProgram = request.DataStore.GetValue("TaskProgram");
            var taskStartTime = request.DataStore.GetValue("TaskStartTime");

            var taskUsername = string.IsNullOrEmpty(request.DataStore.GetValue("ImpersonationUsername"))
                ? WindowsIdentity.GetCurrent().Name
                : NTHelper.CleanDomain(request.DataStore.GetValue("ImpersonationDomain")) + "\\" + NTHelper.CleanUsername(request.DataStore.GetValue("ImpersonationUsername"));
            var taskPassword = string.IsNullOrEmpty(request.DataStore.GetValue("ImpersonationPassword"))
                ? null
                : request.DataStore.GetValue("ImpersonationPassword");

            if (taskPassword == null)
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "CreateTaskPasswordMissing");

            string workingDirectory = request.DataStore.GetValue("TaskDirectory") == null
                ? FileUtility.GetLocalTemplatePath(request.Info.AppName)
                : FileUtility.GetLocalPath(request.DataStore.GetValue("TaskDirectory"));

            bool isPowerShell = taskProgram.EqualsIgnoreCase("powershell");

            using (TaskService ts = new TaskService())
            {
                TaskCollection tasks = ts.RootFolder.GetTasks(new Regex(taskName));
                foreach (Task task in tasks)
                {
                    if (task.Name.EqualsIgnoreCase(taskName))
                    {
                        if (task.State == TaskState.Running || task.State == TaskState.Queued)
                        {
                            task.Stop();
                            NTHelper.KillProcess("azurebcp");
                        }
                        ts.RootFolder.DeleteTask(taskName);
                    }
                }

                TaskDefinition td = ts.NewTask();
                td.RegistrationInfo.Description = taskDescription;
                td.Settings.Compatibility = TaskCompatibility.V2_1;
                td.RegistrationInfo.Author = taskUsername;
                td.Principal.RunLevel = TaskRunLevel.LUA;
                td.Settings.StartWhenAvailable = true;
                td.Settings.RestartCount = 3;
                td.Settings.RestartInterval = TimeSpan.FromMinutes(3);
                td.Settings.MultipleInstances = TaskInstancesPolicy.IgnoreNew;

                td.Triggers.Add(new DailyTrigger
                {
                    DaysInterval = 1,
                    StartBoundary = DateTime.Parse(taskStartTime)
                });

                string optionalArguments = string.Empty;

                if (isPowerShell)
                    optionalArguments = Path.Combine(workingDirectory, taskFile);
                else
                    taskProgram = taskFile;

                if (isPowerShell)
                {
                    optionalArguments = $"-ExecutionPolicy Bypass -File \"{optionalArguments}\"";
                }

                if (!string.IsNullOrEmpty(taskParameters))
                {
                    optionalArguments += " " + taskParameters;
                }

                td.Actions.Add(new ExecAction(taskProgram, optionalArguments, workingDirectory));
                ts.RootFolder.RegisterTaskDefinition(taskName, td, TaskCreation.CreateOrUpdate, taskUsername, taskPassword, TaskLogonType.Password, null);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}