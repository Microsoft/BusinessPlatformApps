using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Actions.Test.ActionsTest
{
    [TestClass]
    public class CommonTests
    {
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
