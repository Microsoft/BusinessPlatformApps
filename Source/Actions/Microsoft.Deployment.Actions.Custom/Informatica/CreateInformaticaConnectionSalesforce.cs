using System.ComponentModel.Composition;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model.Informatica;

namespace Microsoft.Deployment.Actions.Custom.Informatica
{
    [Export(typeof(IAction))]
    public class CreateInformaticaConnectionSalesforce : BaseAction
    {
        private const string URL_CONNECTIONS = "api/v2/connection";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string username = request.DataStore.GetValue("InformaticaUsername");
            string password = request.DataStore.GetValue("InformaticaPassword");
            RestClient rc = await InformaticaUtility.Initialize(username, password);

            InformaticaConnection ic = new InformaticaConnection
            {
                username = request.DataStore.GetValue("SalesforceUser"),
                password = request.DataStore.GetValue("SalesforcePassword"),
                securityToken = request.DataStore.GetValue("SalesforceToken"),
                serviceUrl = $"https://{request.DataStore.GetValue("SalesforceUrl")}/services/Soap/u/34.0",
                Name = InformaticaUtility.BPST_SOURCE_NAME,
                OrgId = rc.ID,
                ConnectionType = "Salesforce",
                RuntimeEnvironmentId = await InformaticaUtility.GetRuntimeEnvironmentId(rc)
            };

            await rc.Post(URL_CONNECTIONS, JsonConvert.SerializeObject(ic));

            await InformaticaUtility.Logout(rc, username, password);

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}