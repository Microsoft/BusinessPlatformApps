using Microsoft.Deployment.Common.Actions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using System.Dynamic;
using System.Net.Http;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.ErrorCode;

namespace Microsoft.Deployment.Actions.AzureCustom
{
    [Export(typeof(IAction))]
    class CreateSqlConnector : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {

            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            var location = request.DataStore.GetJson("SelectedLocation", "Name");
            var apiConnectionName = request.DataStore.GetValue("ApiConnectionName");

            var connectionName = apiConnectionName == null ? "sql" : apiConnectionName;

            var connectionString = request.DataStore.GetValue("SqlConnectionString");
            var conn = SqlUtility.GetSqlCredentialsFromConnectionString(connectionString);

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            Microsoft.Azure.Management.Resources.ResourceManagementClient client = new ResourceManagementClient(creds);
            var registeration = await client.Providers.RegisterAsync("Microsoft.Web");

            dynamic payload = new ExpandoObject();
            payload.properties = new ExpandoObject();
            payload.properties.parameterValues = new ExpandoObject();
            payload.properties.parameterValues.sku = "Enterprise";
            payload.properties.displayName = connectionName;
            payload.properties.parameterValues = new ExpandoObject();
            payload.properties.parameterValues.server = conn.Server;
            payload.properties.parameterValues.database = conn.Database;
            payload.properties.parameterValues.password = conn.Password;
            payload.properties.parameterValues.username = conn.Username;
            payload.properties.api = new ExpandoObject();
            payload.properties.api.id = $"subscriptions/{subscription}/providers/Microsoft.Web/locations/{location}/managedApis/sql";
            payload.location = location;

            HttpResponseMessage connection = await new AzureHttpClient(azureToken, subscription, resourceGroup).ExecuteWithSubscriptionAndResourceGroupAsync(HttpMethod.Put,
                $"/providers/Microsoft.Web/connections/{connectionName}", "2016-06-01", JsonUtility.GetJsonStringFromObject(payload));

            var output = await connection.Content.ReadAsStringAsync();

            if (!connection.IsSuccessStatusCode)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromJsonString(await connection.Content.ReadAsStringAsync()),
                    null, DefaultErrorCodes.DefaultErrorCode, "Failed to create connection");
            }

            request.DataStore.AddToDataStore("sqlConnectionName", connectionName);
            return new ActionResponse(ActionStatus.Success);
        }
    }
}
