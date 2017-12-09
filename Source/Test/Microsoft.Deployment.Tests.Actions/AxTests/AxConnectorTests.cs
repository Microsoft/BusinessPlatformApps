using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.Tests.Actions.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Deployment.Tests.Actions.AxTests
{
    [TestClass]
    public class AxConnectorTests
    {
        [TestMethod]
        public void CreateAxConnector()
        {
            var ds = TestManager.GetDataStore().Result;

            var resp = TestManager.ExecuteAction("Microsoft-CreateResourceGroup", ds);

            ds.AddToDataStore("ConnectorName", "dynamicsax");
            ds.AddToDataStore("RequiresConsent", true);

            resp = TestManager.ExecuteAction("Microsoft-CreateConnectorToLogicApp", ds);
            Assert.IsTrue(resp.IsSuccess == true);
        }

        [TestMethod]
        public void GetAxInstances()
        {
            var ds = TestManager.GetDataStore(true).Result;
            var resp = TestManager.ExecuteAction("Microsoft-GetAxInstances", ds);
        }
    }
}
