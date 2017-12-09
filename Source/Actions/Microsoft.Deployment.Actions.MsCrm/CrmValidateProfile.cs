namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Model;
    using Newtonsoft.Json;

    using Microsoft.Deployment.Common.ActionModel;
    using Microsoft.Deployment.Common.Actions;
    using Microsoft.Deployment.Common.Helpers;
    using System.Linq;

    [Export(typeof(IAction))]
    public class CrmValidateProfile : BaseAction
    {
        private RestClient _rc;
        private string _orgUrl;
        private string _token;
        private string _orgId;

        private async Task<List<string>> RetrieveInvalidEntities(string[] entities)
        {
            List<string> result = new List<string>();
            string response = await _rc.Get(MsCrmEndpoints.URL_ENTITIES, "organizationurl=" + _orgUrl);
            MsCrmEntity[] provisionedEntities = JsonConvert.DeserializeObject<MsCrmEntity[]>(response);

            for (int i = 0; i < entities.Length; i++)
            {
                // TODO: This is a hack to avoid bug with systemusermanagermap
                bool found = entities[i].EqualsIgnoreCase("systemusermanagermap");
                // The loop won't execute for the systemusermanagermap
                for (int j = 0; !found && j < provisionedEntities.Length; j++)
                    found = entities[i].EqualsIgnoreCase(provisionedEntities[j].LogicalName);

                if (!found)
                    result.Add(entities[i]);
            }

            return result;
        }

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            _token = request.DataStore.GetJson("MsCrmToken", "access_token");
            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", _token);
            _rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), bearer);

            _orgUrl = request.DataStore.GetValue("OrganizationUrl");
            _orgId = request.DataStore.GetValue("OrganizationId");
            string name = request.DataStore.GetValue("ProfileName") ?? Constants.CrmProfileName;
            string kV = request.DataStore.GetValue("KeyVault");
            //string[] entities = request.DataStore.GetValue("Entities").Split(new[] {',', ' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> entitiesDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.DataStore.GetValue("Entities"));
            string[] entities = new string[entitiesDict.Count];
            int iCount = 0;
            foreach (var entity in entitiesDict) entities[iCount++] = entity.Key;


            MsCrmProfile profile = new MsCrmProfile
            {
                Entities = new MsCrmEntity[entities.ToArray().Length],
                Name = name,
                OrganizationId = _orgId,
                DestinationKeyVaultUri = kV,
                DestinationPrefix = string.Empty,
                DestinationSchemaName = "dbo"
            };

            for (int i = 0; i < profile.Entities.Length; i++)
            {
                MsCrmEntity e = new MsCrmEntity {Type = entities[i]};
                profile.Entities[i] = e;
            }


            List<string> invalidEntities = await RetrieveInvalidEntities(entities);

            if (invalidEntities.Count > 0)
                return new ActionResponse(ActionStatus.Failure, null,
                                          new ArgumentException("The following entities are not provisioned for replication: " + string.Join(", ", invalidEntities)),
                                          "MsCrm_ErrorCreateProfile");

            try
            {
                await _rc.Post(MsCrmEndpoints.URL_PROFILES_VALIDATE, JsonConvert.SerializeObject(profile));
                
                return new ActionResponse(ActionStatus.Success);
            }
            catch (Exception e)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), e, "MsCrm_ErrorCreateProfile");
            }
        }
    }
}