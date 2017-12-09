using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Management.MachineLearning.CommitmentPlans;
using Microsoft.Azure.Management.MachineLearning.CommitmentPlans.Models;
using Microsoft.Azure.Management.MachineLearning.WebServices;
using Microsoft.Azure.Management.MachineLearning.WebServices.Models;
using Microsoft.Azure.Management.MachineLearning.WebServices.Util;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using Microsoft.Rest;
using Microsoft.Rest.Azure;
using Newtonsoft.Json.Linq;
using CommitmentPlan = Microsoft.Azure.Management.MachineLearning.WebServices.Models.CommitmentPlan;
using WebService = Microsoft.Azure.Management.MachineLearning.WebServices.Models.WebService;

namespace Microsoft.Deployment.Actions.AzureCustom.Reddit
{
    [Export(typeof(IAction))]
    public class DeployRedditAzureMLWebServiceFromFile : BaseAction
    {
        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            string azureToken = request.DataStore.GetJson("AzureToken", "access_token");
            string subscription = request.DataStore.GetJson("SelectedSubscription", "SubscriptionId");

            string webserviceFile = request.DataStore.GetValue("WebServiceFile");
            string webserviceName = request.DataStore.GetValue("WebServiceName");
            string commitmentPlanName = request.DataStore.GetValue("CommitmentPlan");
            string resourceGroup = request.DataStore.GetValue("SelectedResourceGroup");
            string storageAccountName = request.DataStore.GetValue("StorageAccountName");
            string storageAccountKey = request.DataStore.GetValue("StorageAccountKey");

            string responseType = request.DataStore.GetValue("IsRequestResponse");
            bool isRequestResponse = false;

            if (responseType != null)
            {
                isRequestResponse = bool.Parse(responseType);
            }

            string sentimentModelUri = request.DataStore.GetValue("SentimentModelUri");
            string sentimentModelAssetName = request.DataStore.GetValue("AssetModelAzureMLName");

            ServiceClientCredentials creds = new TokenCredentials(azureToken);
            AzureMLWebServicesManagementClient client = new AzureMLWebServicesManagementClient(creds);
            AzureMLCommitmentPlansManagementClient commitmentClient = new AzureMLCommitmentPlansManagementClient(creds);
            client.SubscriptionId = subscription;
            commitmentClient.SubscriptionId = subscription;

            // Create commitment plan
            var commitmentPlan = new Azure.Management.MachineLearning.CommitmentPlans.Models.CommitmentPlan();

            commitmentPlan.Sku = new ResourceSku()
            {
                Capacity = 1,
                Name = "S1",
                Tier = "Standard"
            };

            commitmentPlan.Location = "South Central US";
            var createdCommitmentPlan = await commitmentClient.CommitmentPlans.CreateOrUpdateAsync(commitmentPlan, resourceGroup, commitmentPlanName);

            request.Logger.LogResource(request.DataStore, createdCommitmentPlan.Name,
                DeployedResourceType.MlWebServicePlan, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), createdCommitmentPlan.Id, commitmentPlan.Sku.Tier);

            // Get webservicedefinition
            string sqlConnectionString = request.DataStore.GetValueAtIndex("SqlConnectionString", "SqlServerIndex");
            SqlCredentials sqlCredentials;

            string jsonDefinition = File.ReadAllText(request.Info.App.AppFilePath + "/" + webserviceFile);

            if (!string.IsNullOrWhiteSpace(sqlConnectionString))
            {
                sqlCredentials = SqlUtility.GetSqlCredentialsFromConnectionString(sqlConnectionString);
                jsonDefinition = ReplaceSqlPasswords(sqlCredentials, jsonDefinition);
            }

            if (!string.IsNullOrWhiteSpace(sentimentModelUri) && !string.IsNullOrWhiteSpace(sentimentModelAssetName))
            {
                jsonDefinition = ReplaceSentimentModel(sentimentModelUri, sentimentModelAssetName, jsonDefinition);
            }

            // Create WebService - fixed to southcentralus
            WebService webService = ModelsSerializationUtil.GetAzureMLWebServiceFromJsonDefinition(jsonDefinition);

            webService.Properties.StorageAccount = new StorageAccount
            {
                Key = storageAccountKey,
                Name = storageAccountName
            };

            webService.Properties.CommitmentPlan = new CommitmentPlan(createdCommitmentPlan.Id);
            // A little bit of juggling to change the name
            webService = new WebService(webService.Location, webService.Properties, null, webserviceName, webService.Type, webService.Tags);
            webService.Validate();
            WebService result = null;
            try
            {
                result = client.WebServices.CreateOrUpdate(resourceGroup, webserviceName, webService);


                var keys = client.WebServices.ListKeys(resourceGroup, webserviceName);
                var swaggerLocation = result.Properties.SwaggerLocation;
                string url = swaggerLocation.Replace("swagger.json", "jobs?api-version=2.0");

                if (isRequestResponse)
                {
                    url = swaggerLocation.Replace("swagger.json", "execute?api-version=2.0&format=swagger");
                }

                string serviceKey = keys.Primary;

                request.DataStore.AddToDataStore("AzureMLUrl", url);
                request.DataStore.AddToDataStore("AzureMLKey", serviceKey);
            }
            catch (CloudException e)
            {
                return new ActionResponse(ActionStatus.Failure, JsonUtility.GetJObjectFromStringValue(e.Message), e, "DefaultError", ((CloudException)e).Response.Content);
            }
            request.Logger.LogResource(request.DataStore, result.Name,
                DeployedResourceType.MlWebService, CreatedBy.BPST, DateTime.UtcNow.ToString("o"), result.Id);

            return new ActionResponse(ActionStatus.Success);
        }


        private static string ReplaceSqlPasswords(SqlCredentials sqlCredentials, string json)
        {
            JObject obj = JsonUtility.GetJsonObjectFromJsonString(json);
            var nodes = obj["properties"]?["package"]?["nodes"];
            if (nodes != null)
            {
                foreach (var node in nodes.Children())
                {
                    var nodeConverted = node.Children().First();

                    if (nodeConverted.SelectToken("parameters") != null)
                    {
                        if (nodeConverted["parameters"]["Database Server Name"] != null && nodeConverted["parameters"]["Database Server Name"]["value"] != null)
                        {
                            nodeConverted["parameters"]["Database Server Name"]["value"] = sqlCredentials.Server;
                        }

                        if (nodeConverted["parameters"]["Database Name"] != null && nodeConverted["parameters"]["Database Name"]["value"] != null)
                        {
                            nodeConverted["parameters"]["Database Name"]["value"] = sqlCredentials.Database;
                        }

                        if (nodeConverted["parameters"]["Server User Account Name"] != null && nodeConverted["parameters"]["Server User Account Name"]["value"] != null)
                        {
                            nodeConverted["parameters"]["Server User Account Name"]["value"] = sqlCredentials.Username;
                        }

                        if (nodeConverted["parameters"]["Server User Account Password"] != null && nodeConverted["parameters"]["Server User Account Password"]["value"] != null)
                        {
                            nodeConverted["parameters"]["Server User Account Password"]["value"] = sqlCredentials.Password;
                        }
                    }
                }


                if (obj["properties"].SelectToken("parameters") != null)
                {
                    if (obj["properties"]["parameters"]["database server name"] != null && obj["properties"]["parameters"]["database server name"]["value"] != null)
                    {
                        obj["properties"]["parameters"]["database server name"]["value"] = sqlCredentials.Server;
                    }

                    if (obj["properties"]["parameters"]["database name"] != null && obj["properties"]["parameters"]["database name"]["value"] != null)
                    {
                        obj["properties"]["parameters"]["database name"]["value"] = sqlCredentials.Database;
                    }

                    if (obj["properties"]["parameters"]["user name"] != null && obj["properties"]["parameters"]["user name"]["value"] != null)
                    {
                        obj["properties"]["parameters"]["user name"]["value"] = sqlCredentials.Username;
                    }

                    if (obj["properties"]["parameters"]["Server User Account Password"] != null && obj["properties"]["parameters"]["Server User Account Password"]["value"] != null)
                    {
                        obj["properties"]["parameters"]["Server User Account Password"]["value"] = sqlCredentials.Password;
                    }
                }
            }
            return obj.ToString();
        }

        public static string ReplaceSentimentModel(
            string sentimentModelUri, 
            string sentimentModelAssetName, 
            string json
        )
        {
            var obj = JsonUtility.GetJsonObjectFromJsonString(json);
            var jpathExpression = $"$.properties.assets..[?(@.name=='{sentimentModelAssetName}')]";
            var nodes = obj.SelectToken(jpathExpression);
            if (nodes == null)
            {
                return json;
            }
            foreach (var assetNode in nodes)
            {
                assetNode.Parent["locationInfo"]["uri"] = sentimentModelUri;
            }
            return obj.ToString();
        }
    }
}