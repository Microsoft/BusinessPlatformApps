using Microsoft.Deployment.Common;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.PBI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.Common.PBI
{
    [Export(typeof(IAction))]
    public class CreatePBIWorkspace : BaseAction
    {
        private const string PBI_ENDPOINT_GROUPS = "v1.0/myorg/groups";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            try
            {
                var pbiToken = request.DataStore.GetJson("PBIToken", "access_token").ToString();
                var workspaceName = request.DataStore.GetValue("pbiWorkspaceName");

                var postData = new Dictionary<string, string>
                {
                    {"name", workspaceName}
                };

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", pbiToken);
                    var content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync(Constants.PowerBiApiUrl + PBI_ENDPOINT_GROUPS, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        return new ActionResponse(ActionStatus.Failure, null, null, "PBIWorkspaceCreateFailed", response.StatusCode.ToString());
                    }
                    var jsonBody = await response.Content.ReadAsStringAsync();
                    return new ActionResponse(ActionStatus.Success, jsonBody, true);
                }
            }
            catch (Exception ex)
            {
                return new ActionResponse(ActionStatus.Failure, new ActionResponseExceptionDetail("PBIWorkspaceCreateFailed", ex.Message));
            }
        }
    }
}
