using System.Collections.Generic;
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
    public class GetInformaticaAgents : BaseAction
    {
        private const string URL_AGENT = "api/v2/runtimeEnvironment";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string username = request.DataStore.GetValue("InformaticaUsername");
            string password = request.DataStore.GetValue("InformaticaPassword");
            RestClient rc = await InformaticaUtility.Initialize(username, password);

            string response = await rc.Get(URL_AGENT);
            List<InformaticaRuntimeEnvironment> environments = JsonConvert.DeserializeObject<List<InformaticaRuntimeEnvironment>>(response);

            await InformaticaUtility.Logout(rc, username, password);

            return new ActionResponse(ActionStatus.Success, JsonUtility.Serialize<List<InformaticaRuntimeEnvironment>>(environments));
        }
    }
}