using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Enums;
using System;
using System.Data.SqlClient;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    class ADFDeployLinkedServices : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string sfUsername = request.DataStore.GetValue("SalesforceUser");
            string sfPassword = request.DataStore.GetValue("SalesforcePassword");
            string sfToken = request.DataStore.GetValue("SalesforceToken");
            string sfUrl = request.DataStore.GetValue("SalesforceUrl");

            string fullServerUrl = request.DataStore.GetValue("SalesforceBaseUrl");
            string connString = request.DataStore.GetValue("SqlConnectionString");
            string emails = request.DataStore.GetValue("EmailAddress");

            string dataFactoryName = resourceGroup.Replace("_", string.Empty) + "SalesforceCopyFactory";
            var param = new AzureArmParameterGenerator();

            var sqlConn = new SqlConnectionStringBuilder(connString);

            var sqlFqdn = (sqlConn.DataSource.Contains(":") && sqlConn.DataSource.Contains(",")) ? sqlConn.DataSource.Split(':')[1].Split(',')[0] : sqlConn.DataSource;

            param.AddStringParam("dataFactoryName", dataFactoryName);
            param.AddStringParam("sqlServerFullyQualifiedName", sqlFqdn);
            param.AddStringParam("sqlServerUsername", sqlConn.UserID);
            param.AddStringParam("targetDatabaseName", sqlConn.InitialCatalog);
            param.AddStringParam("salesforceUsername", sfUsername);
            param.AddStringParam("subscriptionId", subscription);
            param.AddStringParam("environmentUrl", sfUrl);
            param.AddParameter("salesforcePassword", "securestring", sfPassword);
            param.AddParameter("sqlServerPassword", "securestring", sqlConn.Password);
            param.AddParameter("salesforceSecurityToken", "securestring", sfToken);

            var armTemplate = JsonUtility.GetJsonObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/ADF/linkedServices.json")));
            var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());

            armTemplate.Remove("parameters");
            armTemplate.Add("parameters", armParamTemplate["parameters"]);

            if (string.IsNullOrEmpty(emails))
            {
                (armTemplate
                    .SelectToken("resources")[0]
                    .SelectToken("resources") as JArray)
                    .RemoveAt(2);
            }
            else
            {
                var addresses = emails.Split(',');
                List<string> adr = new List<string>();

                foreach (var address in addresses)
                {
                    adr.Add(address);
                }

                var stringTemplate = armTemplate.ToString();

                stringTemplate = stringTemplate.Replace("\"EMAILS\"", JsonConvert.SerializeObject(adr));
                armTemplate = JsonUtility.GetJObjectFromJsonString(stringTemplate);
            }

            SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, token);
            ResourceManagementClient client = new ResourceManagementClient(creds);

            var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
            {
                Properties = new DeploymentPropertiesExtended()
                {
                    Template = armTemplate.ToString(),
                    Parameters = JsonUtility.GetEmptyJObject().ToString()
                }
            };

            var factoryIdenity = new ResourceIdentity
            {
                ResourceProviderApiVersion = "2015-10-01",
                ResourceName = dataFactoryName,
                ResourceProviderNamespace = "Microsoft.DataFactory",
                ResourceType = "datafactories"
            };

            var factory = client.Resources.CheckExistence(resourceGroup, factoryIdenity);

            if (factory.Exists)
            {
                client.Resources.Delete(resourceGroup, factoryIdenity);
            }

            string deploymentName = "SalesforceCopyFactory-linkedServices";

            var validate = client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;
            if (!validate.IsValid)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null,
                    DefaultErrorCodes.DefaultErrorCode, $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");
            }

            var deploymentItem = client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;

            var helper = new DeploymentHelper();

            var response = await helper.WaitForDeployment(client, resourceGroup, deploymentName);

            if (response.IsSuccess)
            {
                //Log data factory
                request.Logger.LogResource(request.DataStore, dataFactoryName,
                    DeployedResourceType.AzureDataFactory, CreatedBy.BPST, DateTime.UtcNow.ToString("o"));
            }

            return await helper.WaitForDeployment(client, resourceGroup, deploymentName);
        }
    }
}