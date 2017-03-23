namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Model;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmStartProfile : BaseAction
    {
        private RestClient _rc;
        //private string _orgUrl;
        private string _token;
        private string _orgId;

        private async Task<string> GetProfileId(string organizationId, string name)
        {
            string response = await _rc.Get(MsCrmEndpoints.URL_PROFILES, $"organizationId={WebUtility.UrlEncode(organizationId)}");

            MsCrmProfile[] profiles = JsonConvert.DeserializeObject<MsCrmProfile[]>(response);

            for (int i = 0; i < profiles.Length; i++)
                if (profiles[i].Name.EqualsIgnoreCase(name))
                    return profiles[i].Id;

            return null;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            _token = request.DataStore.GetJson("MsCrmToken", "access_token");
            _orgId = request.DataStore.GetValue("OrganizationId");
            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", _token);
            _rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), bearer);

            // string profileId = await GetProfileId(_orgId, request.DataStore.GetValue("ProfileName"));
            string profileId = request.DataStore.GetValue("ProfileId");
            try
            {
                string response = await _rc.Post(string.Format(MsCrmEndpoints.URL_PROFILES_ACTIVATE, profileId), string.Empty);
                MsCrmProfile validatedProfile = JsonConvert.DeserializeObject<MsCrmProfile>(response);

                return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
            }
            catch (Exception e)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), e, "MsCrm_ErrorCreateProfile");
            }
        }
    }
}