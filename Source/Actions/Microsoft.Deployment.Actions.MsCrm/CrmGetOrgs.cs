using System.Diagnostics;
using System.Threading;

namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.ComponentModel.Composition;
    using System.Dynamic;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmGetOrgs : BaseAction
    {

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("MsCrmToken", "access_token");
            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", token);

            RestClient _rc = new RestClient(MsCrmEndpoints.ENDPOINT, bearer);
            string response = await _rc.Get(MsCrmEndpoints.URL_ORGANIZATIONS);

            MsCrmOrganization[] orgs = JsonConvert.DeserializeObject<MsCrmOrganization[]>(response);

            // This is a bit of a dance to accomodate ActionResponse and its need for a JObject
            response = JsonConvert.SerializeObject(orgs);

            return string.IsNullOrWhiteSpace(response)
                ? new ActionResponse(ActionStatus.Failure, new JObject(), "MsCrm_NoOrgs")
                : new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(response));
        }
    }
}