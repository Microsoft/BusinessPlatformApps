using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Actions.Test.TestHelpers;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Enums;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.Deployment.Common.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class CommonTests
    {
        [TestMethod]
        public void SetConfigValuesInSql_Success()
        {
            var dataStore = TestHarness.GetCommonDataStoreWithSql().Result;
            dataStore.AddToDataStore("SqlServerIndex", 0, DataStoreType.Any);
            dataStore.AddToDataStore("Customize", "SqlGroup", "SolutionTemplate", DataStoreType.Public);
            dataStore.AddToDataStore("Customize", "SqlSubGroup", "System Center", DataStoreType.Public);
            dataStore.AddToDataStore("Customize", "SqlEntryName", "endpointcompliancetarget", DataStoreType.Public);
            dataStore.AddToDataStore("Customize", "SqlEntryValue", "0.99", DataStoreType.Public);

            dataStore.AddToDataStore("Customize1", "SqlGroup", "SolutionTemplate", DataStoreType.Public);
            dataStore.AddToDataStore("Customize1", "SqlSubGroup", "System Center", DataStoreType.Public);
            dataStore.AddToDataStore("Customize1", "SqlEntryName", "healthevaluationtarget", DataStoreType.Public);
            dataStore.AddToDataStore("Customize1", "SqlEntryValue", "0.99", DataStoreType.Public);

            dataStore.AddToDataStore("Customize3", "SqlGroup", "SolutionTemplate", DataStoreType.Public);
            dataStore.AddToDataStore("Customize3", "SqlSubGroup", "System Center", DataStoreType.Public);
            dataStore.AddToDataStore("Customize3", "SqlEntryName", "healthevaluationtarget", DataStoreType.Public);
            dataStore.AddToDataStore("Customize3", "SqlEntryValue", "120", DataStoreType.Public);

            SqlCredentials creds = new SqlCredentials()
            {
                Server = Credential.Instance.Sql.Server,
                Username = Credential.Instance.Sql.Username,
                Password = Credential.Instance.Sql.Password,
                Authentication = SqlAuthentication.SQL,
                Database = TestHarness.CurrentDatabase
            };

            TestHarness.RunSqlCommandWithoutTransaction(creds, "CREATE TABLE [dbo].[testTable]" +
                                                                "(" +
                                                                 "id                     INT IDENTITY(1, 1) NOT NULL," +
                                                                  "configuration_group    VARCHAR(150) NOT NULL," +
                                                                  "configuration_subgroup VARCHAR(150) NOT NULL," +
                                                                  "name                   VARCHAR(150) NOT NULL," +
                                                                  "value                  VARCHAR(max) NULL,    " +
                                                                  "visible                BIT NOT NULL DEFAULT 0" +
                                                                ");");

            dataStore.AddToDataStore("SqlConfigTable", "testTable");

            var response = TestHarness.ExecuteAction("Microsoft-SetConfigValueInSql", dataStore);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        [TestMethod]
        public void WranglePBIXSuccess()
        {
            ActionResponse sqlResponse = GetSqlPagePayload();

            var dataStore = new DataStore();

            dataStore.AddToDataStore("SqlServerIndex", 0, DataStoreType.Any);
            dataStore.AddToDataStore("SqlScriptsFolder", "Service/Database/Cleanup", DataStoreType.Any);
            dataStore.AddToDataStore("SqlConnectionString", (sqlResponse.Body as JObject)["value"].ToString(), DataStoreType.Private);
            dataStore.AddToDataStore("FileName", "SCCMSolutionTemplate.pbix");
            var response = TestHarness.ExecuteAction("Microsoft-WranglePBI", dataStore);

            Assert.IsTrue(response.Status == ActionStatus.Success);
        }

        private ActionResponse GetSqlPagePayload()
        {
            var dataStore = new DataStore();

            dynamic sqlPayload = new ExpandoObject();
            sqlPayload.SqlCredentials = new ExpandoObject();
            sqlPayload.SqlCredentials.Server = ".windows.database.net";
            sqlPayload.SqlCredentials.AuthType = "azuresql";
            sqlPayload.SqlCredentials.User = "";
            sqlPayload.SqlCredentials.Password = "";
            sqlPayload.SqlCredentials.Database = "";

            dataStore.AddObjectDataStore("SqlCredentials", JsonUtility.GetJObjectFromObject(sqlPayload), DataStoreType.Any);

            ActionResponse sqlResponse = TestHarness.ExecuteAction("Microsoft-GetSqlConnectionString", dataStore);
            Assert.IsTrue(sqlResponse.Status == ActionStatus.Success);
            return sqlResponse;
        }
    }
}
