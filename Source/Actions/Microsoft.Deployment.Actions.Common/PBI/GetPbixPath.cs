using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;

namespace Microsoft.Deployment.Actions.Common.PBI
{
    [Export(typeof(IAction))]
    public class GetPbixPath : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string[] originalFiles = request.DataStore.GetValue("FileName").Split('|');
            var serverPaths = new string[originalFiles.Length];

            for (int i = 0; i < originalFiles.Length; i++)
            {
                serverPaths[i] = request.Info.ServiceRootUrl + request.Info.ServiceRelativePath + request.Info.App.AppRelativeFilePath + $"/service/PowerBI/{originalFiles[i]}";
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.Serialize<string[]>(serverPaths));
        }
    }
}