namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using Model;
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.Composition;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmGetProfileStatus : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetValue("MsCrmToken");
            string profileId = request.DataStore.GetValue("createdProfileId");

            RestClient rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), new AuthenticationHeaderValue("Bearer", token));

            try
            {
                string response = await rc.Get(MsCrmEndpoints.URL_PROFILES + "/" + profileId, "status=true");
                MsCrmProfile profile = JsonConvert.DeserializeObject<MsCrmProfile>(response);

                bool done = true;
                for (int i=0; done && i<profile.Entities.Length; i++)
                {
                    done = done && profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("done");
                }
                
                if (done)
                    return new ActionResponse(ActionStatus.Success, null);
                else
                    return new ActionResponse(ActionStatus.BatchNoState, null);
            }
            catch (Exception e)
            {
                return new ActionResponse(ActionStatus.Failure, null, e, "MsCrm_ErrorCreateProfile");
            }
        }
    }
}