namespace Microsoft.Deployment.Actions.SQL
{
    using Common.ActionModel;
    using Common.Actions;
    using System;
    using System.ComponentModel.Composition;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class InstallSqlTools : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            ActionResponse response = new ActionResponse(ActionStatus.Success, "");

            string msiLocationName = Path.Combine(request.Info.App.AppFilePath, "Service\\Resources").Replace('/', '\\');
            string[] installSequence = {  $"/i \"{Path.Combine(msiLocationName, "msodbcsql.msi")}\" /quiet /qn /promptrestart IACCEPTMSODBCSQLLICENSETERMS=YES"
                                       };
            try
            {
                using (Process p = new Process())
                {
                    for (int i = 0; i < installSequence.Length; i++)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo("msiexec.exe", installSequence[i]) { WindowStyle = ProcessWindowStyle.Hidden, WorkingDirectory = msiLocationName };
                        p.StartInfo = startInfo;

                        p.Start(); // if this is false, no new msiexec process was created
                        p.WaitForExit();
                        if (p.ExitCode == 0 || p.ExitCode == 1641 || p.ExitCode == 3010)
                            continue;

                        response = new ActionResponse(ActionStatus.Failure, null, null, null, installSequence[i] + $" from location \"{msiLocationName}\" failed to install and exited with error code {p.ExitCode}");
                        break;

                    }
                }
            }
            catch (Exception e)
            {
                response = new ActionResponse(ActionStatus.Failure, null, e, "InstallSQLToolsFailed");
            }

            return response;
        }
    }
}