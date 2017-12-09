using System.Dynamic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Helpers;

namespace Microsoft.Deployment.Tests.Actions.TestHelpers
{
    public class SqlCreds
    {
        public string Server { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Database { get; set; }

        public static string GetSqlPagePayload(string database)
        {
            var dataStore = new DataStore();

            dynamic sqlPayload = new ExpandoObject();
            sqlPayload.SqlCredentials = new ExpandoObject();
            sqlPayload.SqlCredentials.Server = Credential.Instance.Sql.Server;
            sqlPayload.SqlCredentials.AuthType = "azuresql";
            sqlPayload.SqlCredentials.User = Credential.Instance.Sql.Username;
            sqlPayload.SqlCredentials.Password = Credential.Instance.Sql.Password;
            sqlPayload.SqlCredentials.Database = database;

            dataStore.AddObjectDataStore("SqlCredentials", JsonUtility.GetJObjectFromObject(sqlPayload), DataStoreType.Any);

            ActionResponse sqlResponse = TestManager.ExecuteAction("Microsoft-GetSqlConnectionString", dataStore);
            Assert.IsTrue(sqlResponse.Status == ActionStatus.Success);

            return (sqlResponse.Body as JObject)["value"].ToString();
        }
    }
}