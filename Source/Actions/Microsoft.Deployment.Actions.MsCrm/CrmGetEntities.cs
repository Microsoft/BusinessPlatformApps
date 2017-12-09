using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions.MsCrm.Model;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.WebServiceClient;
using Newtonsoft.Json;

namespace Microsoft.Deployment.Common.Actions.MsCrm
{
    [Export(typeof(IAction))]
    public class CrmGetEntities : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            RestClient rc;
            List<string> result = new List<string>();
            string refreshToken = request.DataStore.GetJson("MsCrmToken")["refresh_token"].ToString();
            string organizationUrl = request.DataStore.GetValue("OrganizationUrl");
            Dictionary<string, string> entities_dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.DataStore.GetValue("Entities"));
            List<string> coreObjects = new List<string>();
            foreach (var entity in entities_dict)
            {
                coreObjects.Add(entity.Key);
            }
            
            var crmObjects = new List<string>();

            var crmToken = CrmTokenUtility.RetrieveCrmOnlineToken(refreshToken, request.Info.WebsiteRootUrl, request.DataStore, organizationUrl);

            var token = request.DataStore.GetJson("MsCrmToken", "access_token");
            AuthenticationHeaderValue bearer = new AuthenticationHeaderValue("Bearer", token);
            var orgUrl = request.DataStore.GetValue("OrganizationUrl");
            rc = new RestClient(request.DataStore.GetValue("ConnectorUrl"), bearer);
            string response = await rc.Get(MsCrmEndpoints.URL_ENTITIES, "organizationurl=" + orgUrl);
            MsCrmEntity[] provisionedEntities = JsonConvert.DeserializeObject<MsCrmEntity[]>(response);

            foreach (var entity in provisionedEntities)
            {
                if (!coreObjects.Contains(entity.LogicalName))
                {
                    crmObjects.Add(entity.LogicalName);
                }
            }

            return new ActionResponse(ActionStatus.Success, JsonUtility.Serialize(crmObjects));
        }
    }
}
