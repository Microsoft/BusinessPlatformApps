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
    public class CreateInformaticaConnectionSql : BaseAction
    {
        private const string URL_CONNECTIONS = "api/v2/connection";

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string username = request.DataStore.GetValue("InformaticaUsername");
            string password = request.DataStore.GetValue("InformaticaPassword");
            RestClient rc = await InformaticaUtility.Initialize(username, password);

            bool isWindowsAuth = request.DataStore.GetJson("SqlCredentials", "AuthType").EqualsIgnoreCase("Windows");

            InformaticaConnectionAzureSql ic = new InformaticaConnectionAzureSql
            {
                Name = InformaticaUtility.BPST_TARGET_NAME,
                OrgId = rc.ID,
                ConnectionType = "SqlServer2012",
                Codepage = "UTF-8",
                Schema = "dbo",
                AuthenticationType = isWindowsAuth ? "Windows" : "SqlServer",
                RuntimeEnvironmentId = await InformaticaUtility.GetRuntimeEnvironmentId(rc)
            };

            string databaseName = request.DataStore.GetValue("Database");
            string serverName = request.DataStore.GetValue("Server");

            ic.Database = isWindowsAuth ? databaseName : databaseName + ";EncryptionMethod=SSL;ValidateServerCertificate=true";
            string[] serverAndPort = serverName.Split(',', ':');
            if (serverAndPort.Length > 1)
            {
                ic.port = int.Parse(serverAndPort[1].Trim());
                ic.Host = serverAndPort[0].Trim();
            }
            else
            {
                ic.Host = serverName;
                ic.port = 1433;
            }

            if (isWindowsAuth)
            {
                ic.username = ic.password = null;
            }
            else
            {
                string sqlUsername = request.DataStore.GetValue("Username");
                ic.password = request.DataStore.GetValue("Password");
                ic.username = sqlUsername.Contains("@") ? sqlUsername : sqlUsername + "@" + ic.Host;
            }

            await rc.Post(URL_CONNECTIONS, JsonConvert.SerializeObject(ic));

            await InformaticaUtility.Logout(rc, username, password);

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetEmptyJObject());
        }
    }
}