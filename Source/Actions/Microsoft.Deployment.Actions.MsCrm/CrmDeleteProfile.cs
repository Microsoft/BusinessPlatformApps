namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Model;
    using Newtonsoft.Json;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmDeleteProfile : BaseAction
    {
        private RestClient _rc;
        private string _token;
        private string _orgId;

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            _token = request.DataStore.GetJson("MsCrmToken")["access_token"].ToString();
            _orgId = request.DataStore.GetValue("OrganizationId");
            string name = request.DataStore.GetValue("ProfileName") ?? "bpst-mscrm-profile";

            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", _token);
            _rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), bearer);

            string response = await _rc.Get(MsCrmEndpoints.URL_PROFILES, $"organizationId={WebUtility.UrlEncode(_orgId)}");
            MsCrmProfile[] profiles = JsonConvert.DeserializeObject<MsCrmProfile[]>(response);

            foreach (MsCrmProfile p in profiles)
            {
                if (p.Name.EqualsIgnoreCase(name) && !p.State.EqualsIgnoreCase("3"))
                    await _rc.Delete(MsCrmEndpoints.URL_PROFILES + "/" + p.Id);
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}
