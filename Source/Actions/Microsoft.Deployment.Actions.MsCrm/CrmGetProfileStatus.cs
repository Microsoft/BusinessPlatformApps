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
            string token = request.DataStore.GetJson("MsCrmToken", "access_token");
            string profileId = request.DataStore.GetValue("ProfileId");

            RestClient rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), new AuthenticationHeaderValue("Bearer", token));

            try
            {
                string response = await rc.Get(MsCrmEndpoints.URL_PROFILES + "/" + profileId, "status=true");
                MsCrmProfile profile = JsonConvert.DeserializeObject<MsCrmProfile>(response);

                bool done = true;
                for (int i = 0; done && i < profile.Entities.Length; i++)
                {
                    done = done && (profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("Completed") || profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("CompletedWithFailures"));
                }

                // REMOVE once dynamics crm team fixes InitialSyncState bug
                int countDone = 0;
                int countEntities = profile.Entities.Length;
                int countNotStarted = 0;
                for (int i = 0; i < countEntities; i++)
                {
                    if (profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("Completed") || profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("CompletedWithFailures"))
                        countDone++;
                    if (profile.Entities[i].Status.InitialSyncState.EqualsIgnoreCase("NotStarted"))
                        countNotStarted++;
                }
                bool tempDone = (countDone + countNotStarted == countEntities) && // every entity is either done or not started
                    (countDone > countEntities * 3 / 5); // about 60% or more of the entities are done
                // END REMOVE

                if (done || tempDone) // if (done)
                    return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
                else
                    return new ActionResponse(ActionStatus.BatchNoState, JsonUtility.GetEmptyJObject());

            }
            catch (Exception e)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), e, "MsCrm_ErrorCreateProfile");
            }
        }
    }
}