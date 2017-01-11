using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.AppLoad;
using Microsoft.Deployment.Common.Controller;
using Microsoft.Deployment.Common.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Microsoft.Deployment.Common.Test
{
    [TestClass]
    public class AppTests
    {
        [TestMethod]
        public void GetApps()
        {

            AppFactory appFactory = new AppFactory(true);
            Assert.IsTrue(appFactory.Apps.Count > 0);
        }

        [TestMethod]
        public void DataStoreRetrievalTest()
        {
            DataStore store = new DataStore();
            store.PrivateDataStore = new Dictionary<string, Dictionary<string, JToken>>();
            store.PublicDataStore = new Dictionary<string, Dictionary<string, JToken>>();

            store.PrivateDataStore.Add("TestRoute", new Dictionary<string, JToken>()
            {
                {"TestObject","TestValue" },
                {"TestObject2","TestValue2" },
                {"password","secret" }
            });

            store.PrivateDataStore.Add("TestRoute2", new Dictionary<string, JToken>()
            {
                {"TestObject","TestValue" },
                {"TestObject2","TestValue2" }
            });

            Assert.IsTrue(store.GetAllValues("TestObject").Count() == 2);
            Assert.IsTrue(store.GetAllValues("TestObject2").Count() == 2);
            Assert.IsTrue(store.GetAllValues("password").Count() == 1);
            Assert.IsTrue(store.GetAllDataStoreItems("password").First().DataStoreType == DataStoreType.Private);
            Assert.IsTrue(store.GetAllDataStoreItems("password").First().ValueAsString == "secret");
            Assert.IsTrue(store.GetAllDataStoreItems("password").First().ToString() == "secret");

            var valueNotFound = store.GetValue("valuenothere");
            var valueNotFoundWithRoute = store.GetValue("routethere", "valuenothere");
            Assert.IsNull(valueNotFoundWithRoute);
            Assert.IsNull(valueNotFound);

            store.AddToDataStore("routethere", "valuenothere", "TestValue");
            valueNotFoundWithRoute = store.GetValue("routethere", "valuenothere");
            Assert.IsNotNull(valueNotFoundWithRoute);

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(store.PrivateDataStore);

        }

        [TestMethod]
        public void TestActionsWithObjectTypes()
        {
            AppFactory appFactory = new AppFactory(true);
            Assert.IsTrue(appFactory.Apps.Count > 0);

            var result = appFactory.Actions["Microsoft-MockAction"].ExecuteActionAsync(null).Result;
            Assert.IsTrue(result.Status == ActionStatus.Success);

            var jobject = JObject.FromObject(result);
            Assert.IsNotNull(jobject);
            Assert.IsNotNull(jobject["Body"]["Value"].ToString());
        }

        [TestMethod]
        public void TestActionWithCommonController()
        {
            AppFactory factory = new AppFactory(true);
            CommonControllerModel model = new CommonControllerModel()
            {
                AppFactory = factory
            };
            CommonController commonController = new CommonController(model);
            UserInfo info = new UserInfo();
            info.ActionName = "Microsoft-MockAction";
            info.AppName = "TestApp";
            var result = commonController.ExecuteAction(info, new ActionRequest() { DataStore = new DataStore() }).Result;
            Assert.IsTrue(result.Status == ActionStatus.Success);
        }

        [TestMethod]
        public void TestAzureParam()
        {
          AzureArmParameterGenerator param = new AzureArmParameterGenerator();
            param.AddStringParam("test", "test2");
            param.AddStringParam("test2", "test2");
            var val = param.GetDynamicObject();
        }
    }
}
