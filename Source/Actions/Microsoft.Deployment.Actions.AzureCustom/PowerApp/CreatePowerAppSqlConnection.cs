using System.ComponentModel.Composition;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using Microsoft.Deployment.Common.Model.PowerApp;

namespace Microsoft.Deployment.Actions.AzureCustom.PowerApp
{
    [Export(typeof(IAction))]
    public class CreatePowerAppSqlConnection : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string powerAppEnvironmentId = request.DataStore.GetValue("PowerAppEnvironment");

            if (powerAppEnvironmentId != null)
            {
                AzureHttpClient ahc = new AzureHttpClient(request.DataStore.GetJson("AzureToken", "access_token"));

                string newSqlConnectionId = RandomGenerator.GetRandomHexadecimal(PowerAppUtility.SQL_CONNECTION_ID_LENGTH);
                string sqlConnectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");

                SqlCredentials sqlCredentials = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);

                PowerAppSqlConnection powerAppSqlConnection = new PowerAppSqlConnection(sqlCredentials, powerAppEnvironmentId);

                string body = JsonUtility.Serialize<PowerAppSqlConnection>(powerAppSqlConnection);
                string url = string.Format(PowerAppUtility.URL_POWERAPPS_SQL_CONNECTION, newSqlConnectionId, powerAppEnvironmentId);

                if (await ahc.IsSuccess(HttpMethod.Put, url, body))
                {
                    request.DataStore.AddToDataStore("PowerAppSqlConnectionId", newSqlConnectionId, DataStoreType.Private);
                }
                else
                {
                    PowerAppUtility.SkipPowerApp(request.DataStore);
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }
    }
}