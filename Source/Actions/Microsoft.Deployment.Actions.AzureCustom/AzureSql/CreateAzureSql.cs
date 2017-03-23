using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureSql
{
    [Export(typeof(IAction))]
    public class CreateAzureSql : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            var location = request.DataStore.GetLastValue("SqlLocation") ?? "westus";
            var databaseTier = request.DataStore.GetLastValue("SqlSku") ?? "S1";

            string server = request.DataStore.GetJson("SqlCredentials").SelectToken("Server")?.ToString();
            string user = request.DataStore.GetJson("SqlCredentials").SelectToken("User")?.ToString();
            string password = request.DataStore.GetJson("SqlCredentials").SelectToken("Password")?.ToString();
            var database = request.DataStore.GetJson("SqlCredentials").SelectToken("Database")?.ToString();

            string serverWithoutExtension = server.Replace(".database.windows.net", string.Empty);

            var param = new AzureArmParameterGenerator();
            param.AddStringParam("SqlServerName", serverWithoutExtension);
            param.AddStringParam("SqlDatabaseName", database);
            param.AddStringParam("Username", user);
            param.AddParameter("Password", "securestring", password);
            param.AddStringParam("Sku", databaseTier);
            param.AddStringParam("SqlLocation", location);

            var armTemplate = JsonUtility.GetJObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.ControllerModel.SiteCommonFilePath, "Service/Arm/sqlserveranddatabase.json")));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());
            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, azureToken);
            ResourceManagementClient client = new ResourceManagementClient(creds);

            var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            string deploymentName = "SqlDatabaseDeployment";

            var validate = await client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());
            if (!validate.IsValid)
            {
                return new ActionResponse(
                    ActionStatus.Failure,
                    JsonUtility.GetJObjectFromObject(validate),
                    null,
                    DefaultErrorCodes.DefaultErrorCode,
                    $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = await client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken());

            // Wait for deployment
            while (true)
            {
                Thread.Sleep(5000);
                var status = await client.Deployments.GetAsync(resourceGroup, deploymentName, new CancellationToken());
                var operations = await client.DeploymentOperations.ListAsync(resourceGroup, deploymentName, new DeploymentOperationsListParameters());
                var provisioningState = status.Deployment.Properties.ProvisioningState;

                if (provisioningState == "Accepted" || provisioningState == "Running")
                    continue;

                if (provisioningState == "Succeeded")
                    break;

                var operation = operations.Operations.First(p => p.Properties.ProvisioningState == ProvisioningState.Failed);
                var operationFailed = await client.DeploymentOperations.GetAsync(resourceGroup, deploymentName, operation.OperationId);

                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(operationFailed), null, DefaultErrorCodes.DefaultErrorCode, operationFailed.Operation.Properties.StatusMessage);                
            }

            SqlCredentials credentials = new SqlCredentials()
            {
                Server = server,
                Username = user,
                Password = password,
                Authentication = SqlAuthentication.SQL,
                Database = database
            };

            var connectionStringResponse = SqlUtility.GetConnectionString(credentials);
            return new ActionResponse(ActionStatus.Success, JsonUtility.CreateJObjectWithValueFromObject(connectionStringResponse), true);
        }
    }
}
