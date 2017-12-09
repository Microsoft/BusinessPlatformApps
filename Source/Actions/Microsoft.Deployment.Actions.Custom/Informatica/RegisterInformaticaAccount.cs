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
    public class RegisterInformaticaAccount : BaseAction
    {
        private const string URL_REGISTER = "ma/api/v2/user/register";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string company = request.DataStore.GetValue("InformaticaCompany");
            string nameFirst = request.DataStore.GetValue("InformaticaNameFirst");
            string nameLast = request.DataStore.GetValue("InformaticaNameLast");
            string password = request.DataStore.GetValue("InformaticaPassword");
            string username = request.DataStore.GetValue("InformaticaUsername");

            RestClient rc = await InformaticaUtility.Initialize(username, password, true);

            InformaticaRegistration registration = new InformaticaRegistration(company, nameFirst, nameLast, password, username);

            await rc.Post(URL_REGISTER, JsonConvert.SerializeObject(registration));

            return new ActionResponse(ActionStatus.Success);
        }
    }
}