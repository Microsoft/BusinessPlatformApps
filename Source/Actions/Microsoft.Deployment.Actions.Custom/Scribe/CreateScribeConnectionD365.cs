using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Scribe;

namespace Microsoft.Deployment.Actions.Custom.Scribe
{
    [Export(typeof(IAction))]
    public class CreateScribeConnectionD365 : BaseAction
    {
        private const string CONNECTOR_ID = "E9BD9381-7D29-4E5C-A367-366626A821D9";
        private const string URL_CONNECTIONS = "/v1/orgs/{0}/connections";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            RestClient rc = ScribeUtility.Initialize(request.DataStore.GetValue("ScribeUsername"), request.DataStore.GetValue("ScribePassword"));

            string apiToken = request.DataStore.GetValue("ScribeApiToken");
            string orgId = request.DataStore.GetValue("ScribeOrganizationId");

            await ScribeUtility.InstallConnector(rc, orgId, CONNECTOR_ID);

            ScribeConnection connection = new ScribeConnection
            {
                ConnectorId = CONNECTOR_ID,
                Color = "#FFEA69A6",
                Name = ScribeUtility.BPST_SOURCE_NAME,
                Properties = new List<ScribeKeyValue>()
            };

            // Set authentication
            ScribeKeyValue kvp = new ScribeKeyValue { Key = "DeploymentType", Value = ScribeUtility.AesEncrypt(apiToken, request.DataStore.GetValue("ScribeDeploymentType")) };
            connection.Properties.Add(kvp);
            kvp = new ScribeKeyValue { Key = "Url", Value = ScribeUtility.AesEncrypt(apiToken, request.DataStore.GetValue("ConnectorUrl")) };
            connection.Properties.Add(kvp);
            // Set CRM user name
            kvp = new ScribeKeyValue { Key = "UserId", Value = ScribeUtility.AesEncrypt(apiToken, request.DataStore.GetValue("D365Username")) };
            connection.Properties.Add(kvp);
            // Set CRM user password
            kvp = new ScribeKeyValue { Key = "Password", Value = ScribeUtility.AesEncrypt(apiToken, request.DataStore.GetValue("D365Password")) };
            connection.Properties.Add(kvp);
            // Set CRM pick list
            kvp = new ScribeKeyValue { Key = "DisplayPickListNames", Value = ScribeUtility.AesEncrypt(apiToken, "true") };
            connection.Properties.Add(kvp);
            // Set organization name
            kvp = new ScribeKeyValue { Key = "Organization", Value = ScribeUtility.AesEncrypt(apiToken, request.DataStore.GetValue("OrganizationName")) };
            connection.Properties.Add(kvp);

            await rc.Post(string.Format(CultureInfo.InvariantCulture, URL_CONNECTIONS, orgId), JsonConvert.SerializeObject(connection, Formatting.Indented));

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}