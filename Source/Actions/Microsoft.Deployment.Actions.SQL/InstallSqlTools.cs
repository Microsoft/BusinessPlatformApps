using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class InstallSqlTools : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            ActionResponse response = new ActionResponse(ActionStatus.Success, "");

            try
            {
                string msiLocationName = Path.Combine(request.Info.App.AppFilePath, "Service\\Resources").Replace('/', '\\');
                string[] installSequence = {
                                              $"/i \"{Path.Combine(msiLocationName, "msodbcsql.msi")}\" /quiet /qn /promptrestart IACCEPTMSODBCSQLLICENSETERMS=YES"
                                           };

                using (Process p = new Process())
                {
                    for (int i = 0; i < installSequence.Length; i++)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo("msiexec.exe", installSequence[i]) { WindowStyle = ProcessWindowStyle.Hidden, WorkingDirectory = msiLocationName };
                        p.StartInfo = startInfo;
                        if (p.Start())
                        {
                            p.WaitForExit();
                            if (p.ExitCode == 0 || p.ExitCode == 1641 || p.ExitCode == 3010) continue;
                            response = new ActionResponse(ActionStatus.Failure, installSequence[i] + " failed to install.");
                            break;
                        }

                        response = new ActionResponse(ActionStatus.Failure, "MSIEXEC failed to execute " + installSequence[i]);
                        break;
                    }
                }
            }
            catch
            {
                response = new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "InstallSQLToolsFailed");
            }

            return response;
        }
    }
}