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
    using System.Net;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    [Export(typeof(IAction))]
    public class CrmGetOrganization : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string token = request.DataStore.GetJson("MsCrmToken", "access_token");
            MsCrmOrganization orgObject = new MsCrmOrganization()
            {
                OrganizationUrl = request.DataStore.GetValue("OrganizationUrl"),
                OrganizationId = request.DataStore.GetValue("OrganizationId")
            };

            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", token);

            RestClient rc = new RestClient(MsCrmEndpoints.ENDPOINT, bearer);
            try
            {
                string response = await rc.Get(MsCrmEndpoints.URL_ORGANIZATION_METADATA, $"organizationUrl={WebUtility.UrlEncode(orgObject.OrganizationUrl)}");
                MsCrmOrganization orgDetails = JsonConvert.DeserializeObject<MsCrmOrganization>(response);
                orgObject.ConnectorUrl = orgDetails.ConnectorUrl;
                if (string.IsNullOrEmpty(orgObject.ConnectorUrl))
                    request.Logger.LogEvent("MSCRM-NoConnectorURL", new System.Collections.Generic.Dictionary<string, string> { { orgObject.OrganizationName, orgObject.OrganizationId } });

                request.DataStore.AddToDataStore("ConnectorUrl", orgObject.ConnectorUrl, DataStoreType.Public);

                return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
            }
            catch (Exception e)
            {
                if (e.Message.ToLowerInvariant().Contains("failed authorization"))
                    return new ActionResponse(ActionStatus.Failure, null, e, "MsCrm_Unauthorized");
                
                return new ActionResponse(ActionStatus.Failure, null, e, "MsCrm_MetadataError");
            }
        }
    }
}