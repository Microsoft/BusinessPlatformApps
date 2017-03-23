using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;

namespace Microsoft.Deployment.Actions.SQL
{
    [Export(typeof(IAction))]
    public class ValidateAndGetWritableDatabases : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string server = request.DataStore.GetJson("SqlCredentials", "Server");
            string user = request.DataStore.GetJson("SqlCredentials").SelectToken("User")?.ToString();
            string password = request.DataStore.GetJson("SqlCredentials").SelectToken("Password")?.ToString();
            var auth = request.DataStore.GetJson("SqlCredentials", "AuthType");

            SqlCredentials credentials = new SqlCredentials()
            {
                Server = server,
                Username = user,
                Password = password,
                Authentication = auth.EqualsIgnoreCase("Windows") ? SqlAuthentication.Windows : SqlAuthentication.SQL
            };

            var response = SqlUtility.GetListOfDatabases(credentials, true);
            return response.Count == 0
                ? new ActionResponse(ActionStatus.Failure, JsonUtility.GetEmptyJObject(), "NoDatabasesFound")
                : new ActionResponse(ActionStatus.Success, JsonUtility.CreateJObjectWithValueFromObject(response));
        }
    }
}