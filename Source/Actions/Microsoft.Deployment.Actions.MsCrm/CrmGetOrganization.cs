namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Model;
    using Newtonsoft.Json.Linq;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmGetOrganization : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("MsCrmToken")["access_token"].ToString();
            string orgURL = request.DataStore.GetValue("OrganizationUrl");
            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", token);

            RestClient rc = new RestClient(MsCrmEndpoints.ENDPOINT, bearer);
            string response = await rc.Get(MsCrmEndpoints.URL_ORGANIZATION_METADATA, $"organizationUrl={WebUtility.UrlEncode(orgURL)}");
            // MsCrmOrganization org = JsonConvert.DeserializeObject<MsCrmOrganization>(response);

            return string.IsNullOrWhiteSpace(response) ? new ActionResponse(ActionStatus.Failure, new JObject(), "MsCrm_NoOrg") :
                                                         new ActionResponse(ActionStatus.Success, new JObject(response));
        }
    }
}