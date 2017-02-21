using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class SalesforceTests
    {
        public string sfUsername = Credential.Instance.Salesforce.Username;
        public string sfPassword = Credential.Instance.Salesforce.Password;
        public string sfToken = Credential.Instance.Salesforce.Token;
        public string sqlServer = Credential.Instance.Sql.Server;
        public string sqlUsername = Credential.Instance.Sql.Username;
        public string sqlDatabase = Credential.Instance.Sql.Database;
        public string sqlPassword = Credential.Instance.Sql.Password;
        public DataStore dataStore = null;

        [TestInitialize]
        public void SelectSalesforceMockApp()
        {
            TestHarness.TemplateName = "SalesforceTestApp";
        }

        [TestMethod]
        public void DeployAdfPipelines()
        {
            this.DeployAdfDataset();

            var response = TestHarness.ExecuteAction("Microsoft-ADFDeployPipelines", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);

            GetAdfSliceStatus();
            GetDataPullStatusShouldReturnTableWithRowsAndStatus();
        }

        public void GetDataPullStatusShouldReturnTableWithRowsAndStatus()
        {
            dataStore.AddToDataStore("FinishedActionName", "Microsoft-ADFSliceStatus");
            dataStore.AddToDataStore("isWaiting", "true");
            dataStore.AddToDataStore("SqlServerIndex", "0");
            dataStore.AddToDataStore("TargetSchema", "dbo");

            var response = TestHarness.ExecuteAction("Microsoft-GetDataPullStatus", dataStore);

            Assert.IsTrue((response.Status == ActionStatus.BatchNoState) || (response.Status == ActionStatus.Success));
        }


        public void GetAdfSliceStatus()
        {
            dataStore.AddToDataStore("Azure", "dataFactoryName", dataStore.GetValue("SelectedResourceGroup") + "SalesforceCopyFactory");
            var response = TestHarness.ExecuteAction("Microsoft-ADFSliceStatus", dataStore);

            Assert.IsTrue((response.Status == ActionStatus.BatchNoState) || (response.Status == ActionStatus.Success));
        }

        [TestMethod]
        public void DeployAdfDataset()
        {
            this.DeployAdfLinkedServicesAndSqlCustomizations();

            var response = TestHarness.ExecuteAction("Microsoft-ADFDeployDatasets", dataStore);
            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public void DeployAdfLinkedServicesAndSqlCustomizations()
        {
            this.SalesforceSqlArtefactsDeploysSuccessful();

            this.GetSalesforceCustomizationValues();

            this.CreateDbRelations();

            var response = TestHarness.ExecuteAction("Microsoft-ADFDeployLinkedServices", dataStore);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public void SalesforceSqlArtefactsDeploysSuccessful()
        {
            dataStore = TestHarness.GetCommonDataStoreWithSql().Result;
            dataStore.AddToDataStore("SalesforceUser", this.sfUsername, DataStoreType.Public);
            dataStore.AddToDataStore("SalesforcePassword", this.sfPassword, DataStoreType.Private);
            dataStore.AddToDataStore("SalesforceToken", this.sfToken, DataStoreType.Private);
            dataStore.AddToDataStore("SalesforceUrl", "login.salesforce.com", DataStoreType.Public);
            dataStore.AddToDataStore("ObjectTables", "Opportunity,Account,Lead,Product2,OpportunityLineItem,OpportunityStage,User,UserRole", DataStoreType.Public);

            this.CleanDb(dataStore);

            var response = TestHarness.ExecuteAction("Microsoft-SalesforceGetObjectMetadata", dataStore);

            dataStore.AddObjectDataStore("Objects", JsonUtility.GetJObjectFromObject(response.Body), DataStoreType.Any);

            response = TestHarness.ExecuteAction("Microsoft-SalesforceSqlArtefacts", dataStore);

            dataStore.AddObjectDataStore(JsonUtility.GetJObjectFromObject(response.Body), DataStoreType.Any);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public void RunSalesforceCredentialValidation()
        {
            DataStore dataStore = new DataStore();
            dataStore.AddToDataStore("SalesforceUser", this.sfUsername, DataStoreType.Public);
            dataStore.AddToDataStore("SalesforcePassword", this.sfPassword, DataStoreType.Private);
            dataStore.AddToDataStore("SalesforceToken", this.sfToken, DataStoreType.Private);
            dataStore.AddToDataStore("SalesforceUrl", "https://login.salesforce.com/", DataStoreType.Public);

            var result = TestHarness.ExecuteAction("Microsoft-ValidateSalesforceCredentials", dataStore);
            Assert.IsTrue(result.Status == ActionStatus.Success);
        }

        public void CleanDb(DataStore dataStore)
        {
            dataStore.AddToDataStore("SqlServerIndex", 0, DataStoreType.Any);
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/Cleanup", DataStoreType.Any);

            var response = TestHarness.ExecuteAction("Microsoft-DeploySQLScripts", dataStore);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        public void CreateDbRelations()
        {
            dataStore.AddToDataStore("SqlServerIndex", 0);
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/ADF");

            var response = TestHarness.ExecuteAction("Microsoft-DeploySQLScripts", dataStore);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        public void GetSalesforceCustomizationValues()
        {
            dataStore.AddToDataStore("EmailAddresses", "dave@contoso.com");
            dataStore.AddToDataStore("fiscalMonth", "March");
            dataStore.AddToDataStore("actuals", string.Empty);

            dataStore.AddToDataStore("pipelineFrequency", "Month");
            dataStore.AddToDataStore("pipelineInterval", "1");
            dataStore.AddToDataStore("pipelineType", "PreDeployment");
            dataStore.AddToDataStore("historicalOnly", false);
            dataStore.AddToDataStore("pipelineStart", "");
            dataStore.AddToDataStore("pipelineEnd", "");

            dataStore.AddToDataStore("postDeploymentPipelineFrequency", "Minute");
            dataStore.AddToDataStore("postDeploymentPipelineInterval", "15");
            dataStore.AddToDataStore("postDeploymentPipelineStart", "");
        }

    }
}