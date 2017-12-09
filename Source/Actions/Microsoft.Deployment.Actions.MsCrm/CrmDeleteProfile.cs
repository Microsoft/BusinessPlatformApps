namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using System;
    using System.ComponentModel.Composition;
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Model;
    using Newtonsoft.Json;

    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;

    [Export(typeof(IAction))]
    public class CrmDeleteProfile : BaseAction
    {

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("MsCrmToken", "access_token");
            string orgId = request.DataStore.GetValue("OrganizationId");
            string connectorUrl = request.DataStore.GetValue("ConnectorUrl");

            string name = request.DataStore.GetValue("ProfileName") ?? Constants.CrmProfileName;

            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", token);
            if (string.IsNullOrEmpty(connectorUrl))
                throw new Exception($"Organization {orgId}: null connector URL specified");

            RestClient rc = new RestClient(connectorUrl, bearer);

            string response = await rc.Get(MsCrmEndpoints.URL_PROFILES, $"organizationId={WebUtility.UrlEncode(orgId)}");
            MsCrmProfile[] profiles = JsonConvert.DeserializeObject<MsCrmProfile[]>(response);

            foreach (MsCrmProfile p in profiles)
            {
                if (p.Name.EqualsIgnoreCase(name) && !p.State.EqualsIgnoreCase("3"))
                    await rc.Delete(MsCrmEndpoints.URL_PROFILES + "/" + p.Id);
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}