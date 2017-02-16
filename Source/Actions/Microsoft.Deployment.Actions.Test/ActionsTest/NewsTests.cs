using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Dynamic;
using Microsoft.Deployment.Common.ActionModel;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class NewsTests
    {
        public static string randomString = RandomGenerator.GetRandomLowerCaseCharacters(5);

        [TestMethod]
        public async Task NewsTemplateDeployment()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            #region DeploySQLScripts

             // Deploy SQL Scripts
            dataStore.AddToDataStore("SqlConnectionString", TestHarness.GetSqlPagePayload("NewsTemplateTest"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database");
            var response = await TestHarness.ExecuteActionAsync("Microsoft-DeploySQLScripts", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.Status == ActionStatus.Success);

            #endregion

            #region DeployFunction

            //// Deploy Function
            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionName", "unittestfunction" + TestHarness.RandomCharacters);
            dataStore.AddToDataStore("RepoUrl", "https://github.com/juluczni/AzureFunctionsNewsTemplate");

            response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            // FTP File Function
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployPrivateAssemblyToFunction", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            #endregion

            #region DeployStorageAccount

            //// Create Storage Account
            dataStore.AddToDataStore("DeploymentName", "StorageDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "unitteststorage" + TestHarness.RandomCharacters);
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "false");

            response = await TestHarness.ExecuteActionAsync("Microsoft-CreateAzureStorageAccount", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-WaitForArmDeploymentStatus", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Get key
            response = await TestHarness.ExecuteActionAsync("Microsoft-GetStorageAccountKey", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-WaitForArmDeploymentStatus", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Deploy blob
            dataStore.AddToDataStore("StorageAccountContainer", "newsimages");

            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployStorageAccountContainer", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            #endregion

            #region AMLWEBServiceDeployment

            // Deploy experiments
            dataStore.AddToDataStore("CommitmentPlan", "motestcomm");
            dataStore.AddToDataStore("Replace", "WebServiceFile", "Service/AzureML/Experiments/TopicsWebService.json");
            dataStore.AddToDataStore("Replace", "WebServiceName", "Topics" +TestHarness.RandomCharacters);
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureMLWebServiceFromFile", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.Status == ActionStatus.Success);

            dataStore.CurrentRoutePage = "1";
            dataStore.AddToDataStore("Replace", "WebServiceFile", "Service/AzureML/Experiments/TopicImagesWebService.json");
            dataStore.AddToDataStore("Replace", "WebServiceName", "TopicImages" + TestHarness.RandomCharacters);
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureMLWebServiceFromFile", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.Status == ActionStatus.Success);

            dataStore.CurrentRoutePage = "2";
            dataStore.AddToDataStore("Replace", "WebServiceFile", "Service/AzureML/Experiments/EntityWebService.json");
            dataStore.AddToDataStore("Replace", "WebServiceName", "Entity" + TestHarness.RandomCharacters);
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureMLWebServiceFromFile", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.Status == ActionStatus.Success);
            #endregion

            #region DeployConnectors

            //Deploy Text Analytics Service
            dataStore.AddToDataStore("DeploymentName", "CongitiveServiceDeployText");
            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService");
            dataStore.AddToDataStore("CognitiveServiceType", "TextAnalytics");
            dataStore.AddToDataStore("CognitiveSkuName", "S1");

            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployCognitiveService", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Get Key for Text Analytics
            response = await TestHarness.ExecuteActionAsync("Microsoft-GetCognitiveKey", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Create connector for Text Analytics
            dataStore.AddToDataStore("ConnectorName", "cognitiveservicestextanalytics");

            dynamic payload = new ExpandoObject();
            payload.apiKey = dataStore.GetValue("CognitiveServiceKey");
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "TextAnalytics");
            //response = await TestHarness.ExecuteActionAsync("Microsoft-CreateConnectorToLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            //Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-UpdateBlobStorageConnector", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("KeyNumber", "1");

            //Deploy Bing Cognitive Service
            dataStore.AddToDataStore("DeploymentName", "CongitiveServiceDeployBing");
            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService2");
            dataStore.AddToDataStore("CognitiveServiceType", "Bing.Search");
            dataStore.AddToDataStore("CognitiveSkuName", "S2");
            dataStore.AddToDataStore("CognitiveServiceKey", "");

            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployCognitiveService", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Get Key for Bing
            response = await TestHarness.ExecuteActionAsync("Microsoft-GetCognitiveKey", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //Create connector for Bing News
            dataStore.AddToDataStore("ConnectorName", "bingsearch");

            payload = new ExpandoObject();
            payload.apiKey = dataStore.GetValue("CognitiveServiceKey");
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "bingsearch");
            //response = await TestHarness.ExecuteActionAsync("Microsoft-CreateConnectorToLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            //Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-UpdateBlobStorageConnector", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            //// Create storage account connector
            //dataStore.AddToDataStore("ConnectorName", "azureblob");
            //dataStore.AddToDataStore("ConnectorDisplayName", "azureblob");
            //payload = new ExpandoObject();
            //payload.accountName = dataStore.GetValue("StorageAccountName");
            //payload.accessKey = dataStore.GetValue("StorageAccountKey");
            //payload = JsonUtility.GetJObjectFromObject(payload);
            //dataStore.AddToDataStore("ConnectorPayload", payload);
            ////response = await TestHarness.ExecuteActionAsync("Microsoft-CreateConnectorToLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            ////Assert.IsTrue(response.IsSuccess);
            //response = await TestHarness.ExecuteActionAsync("Microsoft-UpdateBlobStorageConnector", dataStore, "Microsoft-NewsTemplateTest");
            //Assert.IsTrue(response.IsSuccess);

            //Create SQL Connector
            dataStore.AddToDataStore("ConnectorName", "sql");
            payload = new ExpandoObject();
            string connectionString = TestHarness.GetSqlPagePayload("NewsTemplateTest");
            var connectionStringObj = SqlUtility.GetSqlCredentialsFromConnectionString(connectionString);
            payload.authType = "windows";
            payload.database = connectionStringObj.Database;
            payload.password = connectionStringObj.Password;
            payload.server = connectionStringObj.Server;
            payload.username = connectionStringObj.Username;
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "SQLConnector");
            //response = await TestHarness.ExecuteActionAsync("Microsoft-CreateConnectorToLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            //Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-UpdateBlobStorageConnector", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            // Create AML Connector
            dataStore.AddToDataStore("ConnectorName", "azureml");
            dataStore.AddToDataStore("ConnectorDisplayName", "azureml");
            payload = new ExpandoObject();
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            //response = await TestHarness.ExecuteActionAsync("Microsoft-CreateConnectorToLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            //Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-UpdateBlobStorageConnector", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            #endregion

            #region DeployAMLLogicApp
            dataStore.AddToDataStore("DeploymentName", "amllogicapp");
            dataStore.AddToDataStore("LogicAppName", "AmlLogicApp");
            //Create Logic App
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureMLSchedulerLogicApp", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-WaitForArmDeploymentStatus", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            #endregion

            #region LogicAppNewsOrchestrator
            dataStore.AddToDataStore("DeploymentName", "mainlogicapp");
            dataStore.AddToDataStore("LogicAppName", "MainLogicApp");
            dataStore.AddToDataStore("SearchQuery", "Trump");
            dataStore.AddToDataStore("ImageCacheLogicApp", "testname");
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployNewsTemplateLogicApp", dataStore, "Microsoft-NewsTemplateTest"); ;
            Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-WaitForArmDeploymentStatus", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            #endregion
        }


        [TestMethod]

        public async Task TestAzureMLError()
        {
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            // Deploy SQL Scripts
            dataStore.AddToDataStore("SqlConnectionString", TestHarness.GetSqlPagePayload("NewsTemplateTest"));
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database");

            //// Create Storage Account
            dataStore.AddToDataStore("DeploymentName", "StorageDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "unitteststorageamltest");
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "false");

            var response = await TestHarness.ExecuteActionAsync("Microsoft-CreateAzureStorageAccount", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);
            response = await TestHarness.ExecuteActionAsync("Microsoft-WaitForArmDeploymentStatus", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("CommitmentPlan", "motestcomm");
            dataStore.AddToDataStore("Replace", "WebServiceFile", "Service/AzureML/Experiments/TopicsWebService.json");
            dataStore.AddToDataStore("Replace", "WebServiceName", "Topics");
            response = await TestHarness.ExecuteActionAsync("Microsoft-DeployAzureMLWebServiceFromFile", dataStore, "Microsoft-NewsTemplateTest");
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public async Task NewsTemplateE2E()
        {

            //Manual steps - deploy function + deploy cognitive services
            var dataStore = await TestHarness.GetCommonDataStoreWithUserToken();

            //Deploy Azure Function - need to fix DLL issue before the test will work

            dataStore.AddToDataStore("DeploymentName", "FunctionDeploymentTest");
            dataStore.AddToDataStore("FunctionAppHostingPlan", "FunctionPlanName");
            dataStore.AddToDataStore("Name", "unituestrialbpst" + randomString);

            var response = TestHarness.ExecuteAction("Microsoft-DeployAzureFunction", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Deploy Azure storage account

            //Create Storage account
            dataStore.AddToDataStore("DeploymentName", "StorageDeploymentTest");
            dataStore.AddToDataStore("StorageAccountName", "unittesttrialbpst" + randomString);
            dataStore.AddToDataStore("StorageAccountType", "Standard_LRS");
            dataStore.AddToDataStore("StorageAccountEncryptionEnabled", "false");

            response = TestHarness.ExecuteAction("Microsoft-CreateAzureStorageAccount", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Get key
            response = TestHarness.ExecuteAction("Microsoft-GetStorageAccountKey", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Deploy blob
            dataStore.AddToDataStore("StorageAccountContainer", "mycontainer" + randomString);

            response = TestHarness.ExecuteAction("Microsoft-DeployStorageAccountContainer", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Deploy cognitive services
            dataStore.AddToDataStore("DeploymentName", "CongitiveServiceDeployText");
            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService");
            dataStore.AddToDataStore("CognitiveServiceType", "TextAnalytics");
            dataStore.AddToDataStore("CognitiveSkuName", "S1");

            response = TestHarness.ExecuteAction("Microsoft-DeployCognitiveService", dataStore);
            Assert.IsTrue(response.IsSuccess);

            dataStore.AddToDataStore("DeploymentName", "CongitiveServiceDeployBing");
            dataStore.AddToDataStore("CognitiveServiceName", "TestCognitiveService2");
            dataStore.AddToDataStore("CognitiveServiceType", "Bing.Search");
            dataStore.AddToDataStore("CognitiveSkuName", "S2");

            response = TestHarness.ExecuteAction("Microsoft-DeployCognitiveService", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-GetCognitiveKey", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-DeployCognitiveService", dataStore);
            Assert.IsTrue(response.IsSuccess);

            response = TestHarness.ExecuteAction("Microsoft-GetCognitiveKey", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //Image Cache Logic App
            dataStore.AddToDataStore("DeploymentName", "LogicAppDeploymentTest");
            dataStore.AddToDataStore("LogicAppName", "testname");
            dataStore.AddToDataStore("SearchQuery", "microsoft");
            dataStore.AddToDataStore("ConnectorName", "azureblob");
            dataStore.AddToDataStore("ConnectorDisplayName", "azureblob");
            dataStore.AddToDataStore("ImageCacheLogicApp", "testname");

            dynamic payload = new ExpandoObject();
            payload.accountName = "cacheimages";
            payload.accessKey = "bsDCAU00sSqE48QIg+7cXKNhJbG7/0HnMzzl6nN0Y6L2pJoSTFvpHdKlpQPjjayKtks/IDeU2ep1ONPZh7UAKg==";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);

            response = TestHarness.ExecuteAction("Microsoft-CreateAzureStorageAccount", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);
            //response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            //Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-DeployImageCachingLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-WaitForArmDeploymentStatus", dataStore);
            Assert.IsTrue(response.IsSuccess);

            //NewsTemplateLogicApp
            dataStore.AddToDataStore("SiteName", "unituestrialbpst" + randomString);
            dataStore.AddToDataStore("ConnectorName", "bingnews");
            payload = new ExpandoObject();
            payload.apiKey = "a1a17649b8784afd9219fdcf3f945552";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "BingNews");
            //response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("ConnectorName", "cognitiveservicestextanalytics");
            payload = new ExpandoObject();
            payload.apiKey = "488546b19ba040179eaaf172f19196cf";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "TextAnalytics");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("ConnectorName", "sql");
            payload = new ExpandoObject();
            payload.authType = "windows";
            payload.database = "newstemplate";
            payload.password = "PasswordPlaceholder";
            payload.server = "testtemplateworks.database.windows.net";
            payload.username = "pbiadmin";
            payload = JsonUtility.GetJObjectFromObject(payload);
            dataStore.AddToDataStore("ConnectorPayload", payload);
            dataStore.AddToDataStore("ConnectorDisplayName", "InsertIngestTimeStamp");
            response = TestHarness.ExecuteAction("Microsoft-CreateConnectorToLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
            response = TestHarness.ExecuteAction("Microsoft-UpdateBlobStorageConnector", dataStore);
            Assert.IsTrue(response.IsSuccess);
            dataStore.AddToDataStore("LogicAppName", "testname2");
            response = TestHarness.ExecuteAction("Microsoft-DeployNewsTemplateLogicApp", dataStore);
            Assert.IsTrue(response.IsSuccess);
        }
    }
}

