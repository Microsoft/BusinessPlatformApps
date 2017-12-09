using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Actions.Salesforce.Models;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    class ADFDeployDatasets : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            List<Task<ActionResponse>> task = new List<Task<ActionResponse>>();
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");

            string postDeploymentPipelineType = request.DataStore.GetValue("postDeploymentPipelineType");
            string pipelineFrequency = request.DataStore.GetValue("pipelineFrequency");
            string pipelineInterval = request.DataStore.GetValue("pipelineInterval");
            string pipelineType = request.DataStore.GetValue("pipelineType");
            string connString = request.DataStore.GetValue("SqlConnectionString");

            if (!string.IsNullOrWhiteSpace(postDeploymentPipelineType))
            {
                pipelineFrequency = request.DataStore.GetValue("postDeploymentPipelineFrequency");
                pipelineType = postDeploymentPipelineType;
                pipelineInterval = request.DataStore.GetValue("postDeploymentPipelineInterval");
            }

            string adfJsonData = request.DataStore.GetValue("ADFPipelineJsonData");
            var sqlCreds = SqlUtility.GetSqlCredentialsFromConnectionString(connString);
            string dataFactoryName = resourceGroup.Replace("_", string.Empty) + "SalesforceCopyFactory";
            var obj = JsonConvert.DeserializeObject(adfJsonData, typeof(DeserializedADFPayload)) as DeserializedADFPayload;

            foreach (var o in obj.fields)
            {
                var deploymentName = string.Concat("ADFDataset", pipelineType, o.Item1);

                dynamic datasetParam = new AzureArmParameterGenerator();
                datasetParam.AddStringParam("dataFactoryName", dataFactoryName);
                datasetParam.AddStringParam("targetSqlTable", o.Item1);
                datasetParam.AddStringParam("targetSalesforceTable", o.Item1);
                datasetParam.AddStringParam("sliceFrequency", pipelineFrequency);
                datasetParam.AddStringParam("sliceInterval", pipelineInterval);
                datasetParam.AddStringParam("pipelineType", pipelineType);

                var armTemplate = JsonUtility.GetJsonObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/ADF/datasets.json")));
                var armParamTemplate = JsonUtility.GetJObjectFromObject(datasetParam.GetDynamicObject());

                armTemplate.Remove("parameters");
                armTemplate.Add("parameters", armParamTemplate["parameters"]);

                string tableFields = JsonConvert.SerializeObject(o.Item2);
                StringBuilder query = CreateQuery(o, tableFields);
                string stringTemplate = ReplaceTableFieldsAndQuery(tableFields, query, armTemplate);

                var creds = new TokenCloudCredentials(subscription, token);
                var client = new ResourceManagementClient(creds);

                var deployment = new Microsoft.Azure.Management.Resources.Models.Deployment()
                {
                    Properties = new DeploymentPropertiesExtended()
                    {
                        Template = stringTemplate,
                        Parameters = JsonUtility.GetEmptyJObject().ToString()
                    }
                };

                var validate = client.Deployments.ValidateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;
                if (!validate.IsValid)
                {
                    return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromObject(validate), null,
                        DefaultErrorCodes.DefaultErrorCode, $"Azure:{validate.Error.Message} Details:{validate.Error.Details}");

                }

                task.Add(new Task<ActionResponse>(() =>
                {
                    var deploymentItem = client.Deployments.CreateOrUpdateAsync(resourceGroup, deploymentName, deployment, new CancellationToken()).Result;

                    var helper = new DeploymentHelper();
                    return helper.WaitForDeployment(client, resourceGroup, deploymentName).Result;
                }));
            }

            foreach (var t in task)
            {
                t.Start();
            }

            var results = Task.WhenAll(task.ToArray());

            foreach (var t in results.Result)
            {
                if (t.Status != ActionStatus.Success)
                {
                    return new ActionResponse(ActionStatus.Failure, t.ExceptionDetail.FriendlyErrorMessage);
                }
            }

            return new ActionResponse(ActionStatus.Success);
        }

        // Populate the template with the required fields and query
        private string ReplaceTableFieldsAndQuery(string tableFields, StringBuilder query, JObject armTemplate)
        {
            var stringTemplate = armTemplate.ToString();
            stringTemplate = stringTemplate.Replace("\"SQLTABLEDEFINITION\"", tableFields);
            stringTemplate = stringTemplate.Replace("\"SALESFORCETABLEDEFINITION\"", tableFields);
            stringTemplate = stringTemplate.Replace("\"SALESFORCEQUERY\"", query.ToString());
            return stringTemplate;
        }

        // Create the object specific query 
        private StringBuilder CreateQuery(Field o, string tableFields)
        {
            StringBuilder query = new StringBuilder();
            query.Append("\"$$Text.Format('SELECT");
            foreach (var item in o.Item2)
            {
                query.Append(" " + item.name + ",");
            }
            query.Remove(query.Length - 1, 1);

            if (tableFields.Contains("CreatedDate"))
            {
                if (tableFields.Contains("IsDeleted"))
                {
                    query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = 1 OR IsDeleted = 0) AND ((CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}) OR (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}))', WindowStart,WindowEnd)\"");
                }
                else
                {
                    query.Append(" FROM " + o.Item1 + " WHERE (CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}) OR (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                }
            }
            else
            {
                if (tableFields.Contains("IsDeleted"))
                {
                    query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = 1 OR IsDeleted = 0) AND (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                }
                else
                {
                    query.Append(" FROM " + o.Item1 + " WHERE LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}', WindowStart,WindowEnd)\"");
                }
            }

            return query;
        }
    }
}