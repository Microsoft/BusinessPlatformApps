using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Azure;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.ErrorCode;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Actions.Salesforce.Helpers;
using Microsoft.Deployment.Actions.Salesforce.Models;

namespace Microsoft.Deployment.Actions.Salesforce
{
    [Export(typeof(IAction))]
    class ADFDeployPipelines : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            List<Task<ActionResponse>> task = new List<Task<ActionResponse>>();
            var token = request.DataStore.GetJson("AzureToken", "access_token");
            var subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");
            var resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string connString = request.DataStore.GetValue("SqlConnectionString");
            string schema = "dbo";
            var coreObjects = request.DataStore.GetValue("ObjectTables").SplitByCommaSpaceTabReturnList();

            string postDeploymentPipelineType = request.DataStore.GetValue("postDeploymentPipelineType");
            string pipelineFrequency = request.DataStore.GetValue("pipelineFrequency");
            string pipelineInterval = request.DataStore.GetValue("pipelineInterval");
            string pipelineType = request.DataStore.GetValue("pipelineType");
            string pipelineStart = request.DataStore.GetValue("pipelineStart");
            string pipelineEnd = request.DataStore.GetValue("pipelineEnd");

            bool historicalOnly = Convert.ToBoolean(request.DataStore.GetValue("historicalOnly"));

            string dataFactoryName = resourceGroup.Replace("_", string.Empty) + "SalesforceCopyFactory";

            if (!string.IsNullOrWhiteSpace(postDeploymentPipelineType))
            {
                pipelineFrequency = request.DataStore.GetValue("postDeploymentPipelineFrequency");
                pipelineType = postDeploymentPipelineType;
                pipelineInterval = request.DataStore.GetValue("postDeploymentPipelineInterval");
                pipelineStart = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                pipelineEnd = new DateTime(9999, 12, 31, 23, 59, 59).ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrWhiteSpace(pipelineStart))
            {
                pipelineStart = DateTime.UtcNow.AddYears(-3).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            if (string.IsNullOrWhiteSpace(pipelineEnd))
            {
                pipelineEnd = DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
            }

            var adfJsonData = request.DataStore.GetValue("ADFPipelineJsonData");

            var obj = JsonConvert.DeserializeObject(adfJsonData, typeof(DeserializedADFPayload)) as DeserializedADFPayload;

            obj = ReorderObjects(obj);

            for (int i = 0; i < obj.fields.Count(); i++)
            {
                var o = obj.fields[i];

                string deploymentName = string.Concat("ADFPipeline", pipelineType, o.Item1);

                var sqlCreds = SqlUtility.GetSqlCredentialsFromConnectionString(connString);
                var param = new AzureArmParameterGenerator();
                param.AddStringParam("dataFactoryName", dataFactoryName);
                param.AddStringParam("targetSqlSchema", schema);
                param.AddStringParam("targetSqlTable", o.Item1.ToLowerInvariant());
                param.AddStringParam("targetSalesforceTable", o.Item1);
                param.AddStringParam("pipelineName", o.Item1 + "_CopyPipeline");
                param.AddStringParam("sqlWritableTypeName", o.Item1.ToLowerInvariant() + "type");
                param.AddStringParam("sqlWriterStoredProcedureName", "spMerge" + o.Item1.ToLowerInvariant());
                param.AddStringParam("pipelineStartDate", pipelineStart);
                param.AddStringParam("pipelineEndDate", pipelineEnd);
                param.AddStringParam("pipelineType", pipelineType);
                param.AddStringParam("sliceFrequency", pipelineFrequency);
                param.AddStringParam("sliceInterval", pipelineInterval);

                var armTemplate = JsonUtility.GetJsonObjectFromJsonString(System.IO.File.ReadAllText(Path.Combine(request.Info.App.AppFilePath, "Service/ADF/pipeline.json")));
                var armParamTemplate = JsonUtility.GetJObjectFromObject(param.GetDynamicObject());

                armTemplate.Remove("parameters");
                armTemplate.Add("parameters", armParamTemplate["parameters"]);

                if (i >= 1)
                {
                    armTemplate = CreatePipelineDependency(pipelineType, obj, armTemplate, i - 1);
                }

                string tableFields = JsonConvert.SerializeObject(o.Item2);

                StringBuilder query;

                if (o.Item1 != "Opportunity" &&
                    o.Item1 != "Lead" &&
                    o.Item1 != "OpportunityLineItem" &&
                    pipelineType == "PreDeployment" &&
                    coreObjects.Contains(o.Item1))
                {
                    query = CreateQuery(o, tableFields, true, pipelineStart, pipelineEnd);
                    armTemplate = CreateOneTimePipeline(armTemplate);
                }
                else
                {
                    query = CreateQuery(o, tableFields, false);
                }

                if (historicalOnly && pipelineType == "PostDeployment")
                {
                    armTemplate = this.PausePipeline(armTemplate);
                }

                string stringTemplate = ReplaceTableFieldsAndQuery(tableFields, query, armTemplate);

                SubscriptionCloudCredentials creds = new TokenCloudCredentials(subscription, token);
                ResourceManagementClient client = new ResourceManagementClient(creds);

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

            return new ActionResponse(ActionStatus.Success, JsonUtility.GetJObjectFromStringValue(dataFactoryName));
        }

        // Reorder the objects so the scheduled pipelines
        // (Opportunity, Lead, OpportunityLineItem) are 
        // dependent on the scheduled ones
        private DeserializedADFPayload ReorderObjects(DeserializedADFPayload obj)
        {
            var opportunity = obj.fields.Where(o => o.Item1 == "Opportunity").FirstOrDefault();
            var leads = obj.fields.Where(o => o.Item1 == "Lead").FirstOrDefault();
            var items = obj.fields.Where(o => o.Item1 == "OpportunityLineItem").FirstOrDefault();

            var objects = obj.fields.ToList();
            objects.Remove(opportunity);
            objects.Remove(leads);
            objects.Remove(items);

            objects.Add(opportunity);
            objects.Add(leads);
            objects.Add(items);

            obj.fields = objects.ToArray();

            return obj;
        }

        //Pause post deployment pipeline
        private JObject PausePipeline(JObject armTemplate)
        {
            (armTemplate
            .SelectToken("resources")[0]
            .SelectToken("properties") as JObject).
            Add("isPaused", "true");

            return armTemplate;
        }

        // Modify existing ARM template to create
        // a one time only pipeline
        public JObject CreateOneTimePipeline(JObject armTemplate)
        {

            (armTemplate
                   .SelectToken("resources")[0]
                   .SelectToken("properties")
                   .SelectToken("activities")[0]
                   .SelectToken("policy") as JObject)
                   .Remove("timeout");

            (armTemplate
                   .SelectToken("resources")[0]
                   .SelectToken("properties")
                   .SelectToken("activities")[0] as JObject)
                   .Remove("scheduler");

            (armTemplate
                .SelectToken("resources")[0]
                .SelectToken("properties") as JObject)
                .Add("pipelineMode", "OneTime");

            // One time pipelines must have the same
            // start and end date
            (armTemplate
                .SelectToken("resources")[0]
                .SelectToken("properties") as JObject)
                .Remove("end");

            var start = armTemplate
                .SelectToken("resources")[0]
                .SelectToken("properties")
                .SelectToken("start").ToString();

            (armTemplate
                .SelectToken("resources")[0]
                .SelectToken("properties") as JObject)
                .Add("end", start);

            return armTemplate;
        }

        // Create dependencies between pipelines so
        // the concurrency limit for smaller Salesforce
        // instances is not broken
        private JObject CreatePipelineDependency(string pipelineType, DeserializedADFPayload obj, JObject armTemplate, int index)
        {
            var currentDataset = pipelineType + "_" + obj.fields[index + 1].Item1 + "_Output";
            var dataset = pipelineType + "_" + obj.fields[index].Item1 + "_Output";
            var dependency = JToken.Parse(string.Concat("{ \"name\": \"", dataset, "\"}"));

            if (currentDataset != "PreDeployment_Opportunity_Output")
            {
                (armTemplate
                .SelectToken("resources")[0]
                .SelectToken("properties")
                .SelectToken("activities")[0]
                .SelectToken("inputs") as JArray).
                Add(dependency);
            }

            return armTemplate;
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
        // Needs refactor, as the methods satisfies a variety of cases
        private StringBuilder CreateQuery(Field o, string tableFields, bool oneTimePipeline, string startTime = null, string endTime = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("\"$$Text.Format('SELECT");
            foreach (var item in o.Item2)
            {
                query.Append(" " + item.name + ",");
            }
            query.Remove(query.Length - 1, 1);

            if (oneTimePipeline)
            {
                if (tableFields.ToLowerInvariant().Contains("createddate"))
                {
                    if (tableFields.ToLowerInvariant().Contains("isdeleted"))
                    {
                        query.Append(" FROM " + o.Item1 + $" WHERE (IsDeleted = FALSE OR IsDeleted = TRUE) AND ((CreatedDate > {startTime} AND CreatedDate <= {endTime}) OR (LastModifiedDate > {startTime} AND LastModifiedDate <= {endTime}))')\"");
                    }
                    else
                    {
                        query.Append(" FROM " + o.Item1 + $" WHERE (CreatedDate > {startTime} AND CreatedDate <= {endTime}) OR (LastModifiedDate > {startTime} AND LastModifiedDate <= {endTime})')\"");
                    }
                }
                else
                {
                    if (tableFields.Contains("isdeleted"))
                    {
                        query.Append(" FROM " + o.Item1 + $" WHERE (IsDeleted = FALSE OR IsDeleted = TRUE) AND (LastModifiedDate > {startTime} AND LastModifiedDate <=  {endTime})')\"");
                    }
                    else
                    {
                        query.Append(" FROM " + o.Item1 + $" WHERE LastModifiedDate > {startTime} AND LastModifiedDate <= {endTime}')\"");
                    }
                }
            }
            else
            {
                if (!tableFields.ToLowerInvariant().Contains("lastmodifieddate"))
                {
                    if (tableFields.ToLowerInvariant().Contains("createddate"))
                    {
                        if (tableFields.ToLowerInvariant().Contains("isdeleted"))
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = FALSE OR IsDeleted = TRUE) AND (CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                        }
                        else
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                        }
                    }
                    else
                    {
                        if (tableFields.ToLowerInvariant().Contains("isdeleted"))
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = FALSE OR IsDeleted = TRUE)', WindowStart,WindowEnd)\"");
                        }
                        else
                        {
                            query.Append(" FROM " + o.Item1 + "')\"");
                        }
                    }
                }
                else
                {
                    if (tableFields.ToLowerInvariant().Contains("createddate"))
                    {
                        if (tableFields.ToLowerInvariant().Contains("isdeleted"))
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = FALSE OR IsDeleted = TRUE) AND ((CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}) OR (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}))', WindowStart,WindowEnd)\"");
                        }
                        else
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (CreatedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND CreatedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}) OR (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                        }
                    }
                    else
                    {
                        if (tableFields.ToLowerInvariant().Contains("isdeleted"))
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE (IsDeleted = FALSE OR IsDeleted = TRUE) AND (LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ})', WindowStart,WindowEnd)\"");
                        }
                        else
                        {
                            query.Append(" FROM " + o.Item1 + " WHERE LastModifiedDate > {0:yyyy-MM-ddTHH:mm:sssZ} AND LastModifiedDate <= {1:yyyy-MM-ddTHH:mm:sssZ}', WindowStart,WindowEnd)\"");
                        }
                    }
                }
            }

            return query;
        }
    }
}